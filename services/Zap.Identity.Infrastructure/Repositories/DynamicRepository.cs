using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using Zap.Identity.Application.Interfaces;
using Zap.Identity.Application.DTOs;
using Zap.Identity.Infrastructure.Persistence;

namespace Zap.Identity.Infrastructure.Repositories;

public class DynamicRepository : IDynamicRepository
{
    private readonly IMongoClient _mongoClient;
    private readonly DatabaseSettings _dbSettings;
    private readonly FilterDefinitionBuilder<BsonDocument> _fb = Builders<BsonDocument>.Filter;

    public DynamicRepository(IMongoClient mongoClient, IOptions<DatabaseSettings> dbSettings)
    {
        _mongoClient = mongoClient;
        _dbSettings = dbSettings.Value;
    }

    private IMongoDatabase GetDatabase(string collectionName)
    {
        if (collectionName.Contains("Discount") || collectionName.Equals("Category") || collectionName.Equals("Product") || collectionName.Contains("Image") || collectionName.Contains("Translate"))
        {
            return _mongoClient.GetDatabase("SinglePoint_en");
        }

        string dbKey = "Identity"; 
        if (collectionName.StartsWith("System")) dbKey = "System";
        else if (collectionName.StartsWith("Order") || collectionName.StartsWith("Cart")) dbKey = "Orders";
        else if (collectionName.StartsWith("Inventory") || collectionName.StartsWith("Stock") || collectionName.StartsWith("Purchase")) dbKey = "Warehouse";
        else if (collectionName.StartsWith("CashDrawer")) dbKey = "Payment";
        else if (collectionName.StartsWith("ClockIn") || collectionName.StartsWith("Employee")) dbKey = "Hr";

        if (!_dbSettings.Databases.TryGetValue(dbKey, out var dbName))
        {
            dbName = _dbSettings.DatabaseName;
        }

        return _mongoClient.GetDatabase(dbName);
    }

    private FilterDefinition<BsonDocument> GetUserFilter(string collectionName, string userGuid)
    {
        if (collectionName.StartsWith("System")) return _fb.Empty;
        if (collectionName.Equals("Product", StringComparison.OrdinalIgnoreCase)) return _fb.Eq("EmpGuid", userGuid);
        return _fb.Or(_fb.Eq("UserGuid", userGuid), _fb.Eq("EmpGuid", userGuid));
    }

    private object ParseValue(object? value, int valueType)
    {
        if (value == null) return BsonNull.Value;
        if (value is JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String: 
                    var s = element.GetString() ?? "";
                    if (valueType == 5) return ParseArray(s);
                    return s;
                case JsonValueKind.Number:
                    if (element.TryGetInt32(out int i)) return i;
                    return element.GetDouble();
                case JsonValueKind.True: return true;
                case JsonValueKind.False: return false;
                case JsonValueKind.Array:
                    return element.EnumerateArray().Select(x => ParseValue(x, 0)).ToList();
            }
        }
        string stringVal = value.ToString() ?? "";
        if (valueType == 5) return ParseArray(stringVal);
        switch (valueType)
        {
            case 2: if (int.TryParse(stringVal, out int i)) return i; break;
            case 3: if (double.TryParse(stringVal, out double d)) return d; break;
            case 4: if (bool.TryParse(stringVal, out bool b)) return b; break;
        }
        return stringVal;
    }

    private List<object> ParseArray(string val)
    {
        var cleanVal = val.Replace("\\\"", "\"");
        try {
            var list = JsonSerializer.Deserialize<List<object>>(cleanVal);
            return list?.Select(x => ParseValue(x, 0)).ToList() ?? new List<object>();
        } catch {
            return val.Trim('[', ']').Split(',').Select(x => x.Trim().Trim('"').Trim('\'')).Where(x => !string.IsNullOrEmpty(x)).Select(x => (object)x).ToList();
        }
    }

    private BsonValue ToBsonValue(object val)
    {
        if (val is string s && s.Length == 24 && Regex.IsMatch(s, @"^[0-9a-fA-F]+$"))
        {
            try { return ObjectId.Parse(s); } catch { }
        }
        return BsonValue.Create(val);
    }

    private List<BsonDocument> BuildPipeline(string collectionName, FilterDefinition<BsonDocument> filter, string? language, int limit, int skip, string? sortBy, bool sortDescending)
    {
        var langCode = string.IsNullOrEmpty(language) ? "vi" : (language.Length >= 2 ? language.Substring(0, 2).ToLower() : "vi");
        var pipeline = new List<BsonDocument>();
        pipeline.Add(new BsonDocument("$match", filter.Render(BsonSerializer.SerializerRegistry.GetSerializer<BsonDocument>(), BsonSerializer.SerializerRegistry)));

        var translates = new Dictionary<string, (string table, string foreignKey)> {
            { "CustomerDiscounts", ("TranslateCustomerDiscounts", "CustomerDiscountGuid") },
            { "Category", ("TranslateCategory", "ReferenceId") },
            { "Product", ("TranslateProduct", "ReferenceId") }
        };

        var images = new Dictionary<string, (string table, string foreignKey)> {
            { "CustomerDiscounts", ("DiscountsImages", "DiscountGuid") },
            { "Product", ("ProductImages", "ProductGuid") }
        };

        // 1. Join Translate
        if (translates.TryGetValue(collectionName, out var t))
        {
            pipeline.Add(new BsonDocument("$lookup", new BsonDocument {
                { "from", t.table },
                { "let", new BsonDocument("mainId", "$_id") },
                { "pipeline", new BsonArray {
                    new BsonDocument("$match", new BsonDocument {
                        { "$expr", new BsonDocument("$eq", new BsonArray { "$" + t.foreignKey, "$$mainId" }) },
                        { "Code", langCode }
                    })
                }},
                { "as", "Translations" }
            }));
            pipeline.Add(new BsonDocument("$addFields", new BsonDocument("Translation", new BsonDocument("$arrayElemAt", new BsonArray { "$Translations", 0 }))));
        }

        // 2. Join Images
        if (images.TryGetValue(collectionName, out var img))
        {
            pipeline.Add(new BsonDocument("$lookup", new BsonDocument {
                { "from", img.table },
                { "let", new BsonDocument("mainId", "$_id") },
                { "pipeline", new BsonArray {
                    new BsonDocument("$match", new BsonDocument {
                        { "$expr", new BsonDocument("$eq", new BsonArray { "$" + img.foreignKey, "$$mainId" }) },
                        { "Visible", 1 }
                    }),
                    new BsonDocument("$sort", new BsonDocument("OrderNo", 1))
                }},
                { "as", "Images" }
            }));
        }

        // 3. Mapping fields thông minh
        var addFields = new BsonDocument();
        if (translates.ContainsKey(collectionName))
        {
            // $ifNull only accepts 2 args, so we nest them for multiple fallbacks
            addFields.Add("DiscountName", new BsonDocument("$ifNull", new BsonArray { 
                "$Translation.Title", 
                new BsonDocument("$ifNull", new BsonArray { 
                    "$DiscountName", 
                    new BsonDocument("$ifNull", new BsonArray { "$Title", "$Name" })
                })
            }));
            addFields.Add("Name", new BsonDocument("$ifNull", new BsonArray { 
                "$Translation.Title", 
                new BsonDocument("$ifNull", new BsonArray { 
                    "$Translation.Name", 
                    new BsonDocument("$ifNull", new BsonArray { 
                        "$Name", 
                        new BsonDocument("$ifNull", new BsonArray { "$DiscountName", "$Title" })
                    })
                })
            }));
            addFields.Add("Description", new BsonDocument("$ifNull", new BsonArray { "$Translation.Description", "$Description" }));
            addFields.Add("TermsConditions", "$Translation.TermsConditions");
        }
        if (images.ContainsKey(collectionName))
        {
            addFields.Add("ImageUrl", new BsonDocument("$let", new BsonDocument {
                { "vars", new BsonDocument("firstImg", new BsonDocument("$arrayElemAt", new BsonArray { "$Images", 0 })) },
                { "in", "$$firstImg.Url" }
            }));
        }

        // Xử lý bảng System: Tự động map Title_vi/Title_en sang Title dựa trên langCode
        if (collectionName.StartsWith("System"))
        {
            string suffix = "_" + langCode; // _vi hoặc _en
            // $ifNull only accepts 2 args, nest for multiple fallbacks
            addFields.Add("Title", new BsonDocument("$ifNull", new BsonArray { "$" + "Title" + suffix, "$Title" }));
            addFields.Add("Name", new BsonDocument("$ifNull", new BsonArray { 
                "$" + "Title" + suffix, 
                new BsonDocument("$ifNull", new BsonArray { 
                    "$" + "Name" + suffix, 
                    new BsonDocument("$ifNull", new BsonArray { "$Name", "$Title" })
                })
            }));
        }
        
        if (addFields.ElementCount > 0) pipeline.Add(new BsonDocument("$addFields", addFields));

        // 4. Cleanup & Sorting
        pipeline.Add(new BsonDocument("$project", new BsonDocument("Translations", 0).Add("Translation", 0).Add("Images", 0)));

        if (!string.IsNullOrEmpty(sortBy)) pipeline.Add(new BsonDocument("$sort", new BsonDocument(sortBy, sortDescending ? -1 : 1)));
        else pipeline.Add(new BsonDocument("$sort", new BsonDocument("CreateDate", -1).Add("OrderNo", 1)));

        if (skip > 0) pipeline.Add(new BsonDocument("$skip", skip));
        if (limit > 0) pipeline.Add(new BsonDocument("$limit", limit));

        return pipeline;
    }

    public async Task<IEnumerable<BsonDocument>> GetAllAsync(string collectionName, string userGuid, List<FilterItemDto>? filters = null, int limit = 100, int skip = 0, string? sortBy = null, bool sortDescending = false, string? language = "vi")
    {
        var db = GetDatabase(collectionName);
        var collection = db.GetCollection<BsonDocument>(collectionName);
        var finalFilter = GetUserFilter(collectionName, userGuid);
        bool isCheckAll = false;

        if (filters != null && filters.Any())
        {
            var extraFilters = new List<FilterDefinition<BsonDocument>>();
            foreach (var item in filters)
            {
                if (string.IsNullOrEmpty(item.SearchKey) || item.Value == null || item.Value.ToString() == "All") 
                {
                    if (item.SearchKey == "IsUseCheckAll") isCheckAll = true;
                    continue;
                }
                
                var val = ParseValue(item.Value, item.ValueType);
                if (item.SearchKey == "_id")
                {
                    if (item.SearchQueryType == 1 && val is string sid) { extraFilters.Add(_fb.Eq("_id", ToBsonValue(sid))); continue; }
                    if (item.SearchQueryType == 12 && val is System.Collections.IEnumerable idList) {
                        var bList = new List<BsonValue>();
                        foreach (var o in idList) bList.Add(ToBsonValue(o));
                        extraFilters.Add(_fb.In("_id", bList));
                        continue;
                    }
                }

                switch (item.SearchQueryType)
                {
                    case 1: extraFilters.Add(_fb.Eq(item.SearchKey, ToBsonValue(val))); break;
                    case 7: extraFilters.Add(_fb.Regex(item.SearchKey, new BsonRegularExpression(val.ToString(), "i"))); break;
                    case 12: 
                        if (val is System.Collections.IEnumerable list) {
                            var bList = new List<BsonValue>();
                            foreach (var o in list) bList.Add(ToBsonValue(o));
                            extraFilters.Add(_fb.In(item.SearchKey, bList));
                        } else extraFilters.Add(_fb.Eq(item.SearchKey, ToBsonValue(val)));
                        break;
                    default: extraFilters.Add(_fb.Eq(item.SearchKey, ToBsonValue(val))); break;
                }
            }
            if (isCheckAll) extraFilters.Add(_fb.Eq("Visible", 1));
            if (extraFilters.Any()) finalFilter = _fb.And(finalFilter, _fb.And(extraFilters));
        }

        var pipeline = BuildPipeline(collectionName, finalFilter, language, limit, skip, sortBy, sortDescending);
        return await collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
    }

    public async Task<BsonDocument?> GetByIdAsync(string collectionName, string id, string userGuid, string? language = "vi")
    {
        var db = GetDatabase(collectionName);
        var collection = db.GetCollection<BsonDocument>(collectionName);
        var idFilter = ToBsonValue(id).IsObjectId ? _fb.Eq("_id", ToBsonValue(id)) : _fb.Eq("_id", id);
        var finalFilter = _fb.And(idFilter, GetUserFilter(collectionName, userGuid));
        
        var pipeline = BuildPipeline(collectionName, finalFilter, language, 1, 0, null, false);
        return await collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
    }

    public async Task<BsonDocument> CreateAsync(string collectionName, BsonDocument document, string userGuid)
    {
        var collection = GetDatabase(collectionName).GetCollection<BsonDocument>(collectionName);
        if (collectionName.Equals("Product", StringComparison.OrdinalIgnoreCase)) document["EmpGuid"] = userGuid;
        else if (!collectionName.StartsWith("System")) document["UserGuid"] = userGuid;
        await collection.InsertOneAsync(document);
        return document;
    }

    public async Task UpdateAsync(string collectionName, string id, BsonDocument document, string userGuid)
    {
        var collection = GetDatabase(collectionName).GetCollection<BsonDocument>(collectionName);
        var filter = _fb.And(ToBsonValue(id).IsObjectId ? _fb.Eq("_id", ToBsonValue(id)) : _fb.Eq("_id", id), GetUserFilter(collectionName, userGuid));
        document.Remove("_id");
        var result = await collection.ReplaceOneAsync(filter, document);
        if (result.MatchedCount == 0) throw new Exception("Not found or no permission.");
    }

    public async Task DeleteAsync(string collectionName, string id, string userGuid)
    {
        var collection = GetDatabase(collectionName).GetCollection<BsonDocument>(collectionName);
        var filter = _fb.And(ToBsonValue(id).IsObjectId ? _fb.Eq("_id", ToBsonValue(id)) : _fb.Eq("_id", id), GetUserFilter(collectionName, userGuid));
        var result = await collection.DeleteOneAsync(filter);
        if (result.DeletedCount == 0) throw new Exception("Not found or no permission.");
    }
}
