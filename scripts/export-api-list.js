const XLSX = require('xlsx');

const apis = [
  // ── Authentication ──────────────────────────────────────────
  { service: 'Authentication', port: 5001, method: 'POST', endpoint: '/api/auth/check-account',         description: 'Check account exists' },
  { service: 'Authentication', port: 5001, method: 'POST', endpoint: '/api/auth/send-otp',              description: 'Send OTP' },
  { service: 'Authentication', port: 5001, method: 'POST', endpoint: '/api/auth/verify-otp',            description: 'Verify OTP' },
  { service: 'Authentication', port: 5001, method: 'POST', endpoint: '/api/auth/login',                 description: 'Login' },
  { service: 'Authentication', port: 5001, method: 'GET',  endpoint: '/api/auth/health',                description: 'Health check' },
  { service: 'Authentication', port: 5001, method: 'POST', endpoint: '/api/otp/send',                   description: 'Send OTP (standalone)' },
  { service: 'Authentication', port: 5001, method: 'POST', endpoint: '/api/otp/verify',                 description: 'Verify OTP (standalone)' },
  { service: 'Authentication', port: 5001, method: 'POST', endpoint: '/api/register/check-account',     description: 'Register - check account' },
  { service: 'Authentication', port: 5001, method: 'POST', endpoint: '/api/register/check-email',       description: 'Register - check email' },
  { service: 'Authentication', port: 5001, method: 'POST', endpoint: '/api/register/check-phone',       description: 'Register - check phone' },
  { service: 'Authentication', port: 5001, method: 'POST', endpoint: '/api/register/check-merchant-url',description: 'Register - check merchant URL' },
  { service: 'Authentication', port: 5001, method: 'POST', endpoint: '/api/register/send-otp',          description: 'Register - send OTP' },
  { service: 'Authentication', port: 5001, method: 'POST', endpoint: '/api/register/check-otp',         description: 'Register - check OTP' },
  { service: 'Authentication', port: 5001, method: 'POST', endpoint: '/api/register/insert-merchant',   description: 'Register - insert merchant' },

  // ── HR ──────────────────────────────────────────────────────
  { service: 'HR',             port: 5002, method: 'GET',  endpoint: '/api/Employees/health',            description: 'Health check' },
  { service: 'HR',             port: 5002, method: 'POST', endpoint: '/api/Employees/list',              description: 'List employees' },
  { service: 'HR',             port: 5002, method: 'GET',  endpoint: '/api/Employees/{id}',              description: 'Get employee by ID' },
  { service: 'HR',             port: 5002, method: 'POST', endpoint: '/api/Employees',                   description: 'Create employee' },
  { service: 'HR',             port: 5002, method: 'PUT',  endpoint: '/api/Employees/{id}',              description: 'Update employee' },

  // ── Customer ─────────────────────────────────────────────────
  { service: 'Customer',       port: 5002, method: 'GET',  endpoint: '/api/Customer/health',             description: 'Health check' },
  { service: 'Customer',       port: 5002, method: 'POST', endpoint: '/api/Customer/list',               description: 'List customers' },
  { service: 'Customer',       port: 5002, method: 'GET',  endpoint: '/api/Customer/{id}',               description: 'Get customer by ID' },
  { service: 'Customer',       port: 5002, method: 'POST', endpoint: '/api/Customer',                    description: 'Create customer' },
  { service: 'Customer',       port: 5002, method: 'PUT',  endpoint: '/api/Customer/{id}',               description: 'Update customer' },
  { service: 'Customer',       port: 5002, method: 'GET',  endpoint: '/api/CustomerGroups/health',       description: 'Health check' },
  { service: 'Customer',       port: 5002, method: 'POST', endpoint: '/api/CustomerGroups/list',         description: 'List customer groups' },
  { service: 'Customer',       port: 5002, method: 'GET',  endpoint: '/api/CustomerGroups/{id}',         description: 'Get customer group by ID' },
  { service: 'Customer',       port: 5002, method: 'POST', endpoint: '/api/CustomerGroups',              description: 'Create customer group' },
  { service: 'Customer',       port: 5002, method: 'PUT',  endpoint: '/api/CustomerGroups/{id}',         description: 'Update customer group' },
  { service: 'Customer',       port: 5002, method: 'GET',  endpoint: '/api/v1/management/customers',     description: 'Management - list customers' },
  { service: 'Customer',       port: 5002, method: 'GET',  endpoint: '/api/v1/management/memberships',   description: 'Management - list memberships' },

  // ── Product ──────────────────────────────────────────────────
  { service: 'Product',        port: 5003, method: 'GET',  endpoint: '/api/products/health',             description: 'Health check' },
  { service: 'Product',        port: 5003, method: 'POST', endpoint: '/api/products/list',               description: 'List products' },
  { service: 'Product',        port: 5003, method: 'GET',  endpoint: '/api/products/{id}',               description: 'Get product by ID' },
  { service: 'Product',        port: 5003, method: 'POST', endpoint: '/api/products',                    description: 'Create product' },
  { service: 'Product',        port: 5003, method: 'PUT',  endpoint: '/api/products/{id}',               description: 'Update product' },
  { service: 'Product',        port: 5003, method: 'POST', endpoint: '/api/categories/list',             description: 'List categories' },
  { service: 'Product',        port: 5003, method: 'GET',  endpoint: '/api/categories/all',              description: 'Get all categories' },
  { service: 'Product',        port: 5003, method: 'GET',  endpoint: '/api/categories/{id}',             description: 'Get category by ID' },
  { service: 'Product',        port: 5003, method: 'POST', endpoint: '/api/categories',                  description: 'Create category' },
  { service: 'Product',        port: 5003, method: 'PUT',  endpoint: '/api/categories/{id}',             description: 'Update category' },
  { service: 'Product',        port: 5003, method: 'POST', endpoint: '/api/brands/list',                 description: 'List brands (paginated)' },
  { service: 'Product',        port: 5003, method: 'GET',  endpoint: '/api/brands',                      description: 'Get all brands' },
  { service: 'Product',        port: 5003, method: 'GET',  endpoint: '/api/brands/{id}',                 description: 'Get brand by ID' },
  { service: 'Product',        port: 5003, method: 'POST', endpoint: '/api/brands',                      description: 'Create brand' },
  { service: 'Product',        port: 5003, method: 'PUT',  endpoint: '/api/brands/{id}',                 description: 'Update brand' },
  { service: 'Product',        port: 5003, method: 'DELETE',endpoint: '/api/brands/{id}',               description: 'Delete brand' },
  { service: 'Product',        port: 5003, method: 'POST', endpoint: '/api/modifiergroups/list',         description: 'List modifier groups (paginated)' },
  { service: 'Product',        port: 5003, method: 'GET',  endpoint: '/api/modifiergroups',              description: 'Get all modifier groups' },
  { service: 'Product',        port: 5003, method: 'POST', endpoint: '/api/modifiergroups',              description: 'Create modifier group' },
  { service: 'Product',        port: 5003, method: 'PUT',  endpoint: '/api/modifiergroups/{id}',         description: 'Update modifier group' },
  { service: 'Product',        port: 5003, method: 'DELETE',endpoint: '/api/modifiergroups/{id}',        description: 'Delete modifier group' },
  { service: 'Product',        port: 5003, method: 'POST', endpoint: '/api/units/list',                  description: 'List units (paginated)' },
  { service: 'Product',        port: 5003, method: 'GET',  endpoint: '/api/units',                       description: 'Get all units' },
  { service: 'Product',        port: 5003, method: 'POST', endpoint: '/api/units',                       description: 'Create unit' },
  { service: 'Product',        port: 5003, method: 'PUT',  endpoint: '/api/units/{id}',                  description: 'Update unit' },
  { service: 'Product',        port: 5003, method: 'DELETE',endpoint: '/api/units/{id}',                 description: 'Delete unit' },
  { service: 'Product',        port: 5003, method: 'POST', endpoint: '/api/Locations/list',              description: 'List locations' },
  { service: 'Product',        port: 5003, method: 'GET',  endpoint: '/api/Locations/{id}',              description: 'Get location by ID' },
  { service: 'Product',        port: 5003, method: 'POST', endpoint: '/api/Locations',                   description: 'Create location' },
  { service: 'Product',        port: 5003, method: 'PUT',  endpoint: '/api/Locations/{id}',              description: 'Update location' },
  { service: 'Product',        port: 5003, method: 'GET',  endpoint: '/api/Locations/provinces',         description: 'Get province list' },
  { service: 'Product',        port: 5003, method: 'POST', endpoint: '/api/menus/list',                  description: 'List menus (paginated)' },
  { service: 'Product',        port: 5003, method: 'GET',  endpoint: '/api/menus',                       description: 'Get all menus' },
  { service: 'Product',        port: 5003, method: 'POST', endpoint: '/api/menus',                       description: 'Create menu' },
  { service: 'Product',        port: 5003, method: 'PUT',  endpoint: '/api/menus/{id}',                  description: 'Update menu' },
  { service: 'Product',        port: 5003, method: 'DELETE',endpoint: '/api/menus/{id}',                 description: 'Delete menu' },
  { service: 'Product',        port: 5003, method: 'GET',  endpoint: '/api/diningoptions',               description: 'Get all dining options' },
  { service: 'Product',        port: 5003, method: 'GET',  endpoint: '/api/diningoptions/{id}',          description: 'Get dining option by ID' },
  { service: 'Product',        port: 5003, method: 'POST', endpoint: '/api/diningoptions',               description: 'Create dining option' },
  { service: 'Product',        port: 5003, method: 'PUT',  endpoint: '/api/diningoptions/{id}',          description: 'Update dining option' },
  { service: 'Product',        port: 5003, method: 'GET',  endpoint: '/api/v1/management/prices',        description: 'Management - list prices' },

  // ── Order ────────────────────────────────────────────────────
  { service: 'Order',          port: 5004, method: 'GET',  endpoint: '/api/Orders/health',               description: 'Health check' },
  { service: 'Order',          port: 5004, method: 'POST', endpoint: '/api/Orders/list',                 description: 'List orders' },
  { service: 'Order',          port: 5004, method: 'GET',  endpoint: '/api/Orders/{id}',                 description: 'Get order by ID' },
  { service: 'Order',          port: 5004, method: 'GET',  endpoint: '/api/v1/management/transactions',  description: 'Management - list transactions' },

  // ── Payment ──────────────────────────────────────────────────
  { service: 'Payment',        port: 5006, method: 'GET',  endpoint: '/api/PaymentTerms/health',         description: 'Health check' },
  { service: 'Payment',        port: 5006, method: 'POST', endpoint: '/api/PaymentTerms/list',           description: 'List payment terms' },
  { service: 'Payment',        port: 5006, method: 'GET',  endpoint: '/api/PaymentTerms/{id}',           description: 'Get payment term by ID' },
  { service: 'Payment',        port: 5006, method: 'POST', endpoint: '/api/PaymentTerms',                description: 'Create payment term' },
  { service: 'Payment',        port: 5006, method: 'PUT',  endpoint: '/api/PaymentTerms/{id}',           description: 'Update payment term' },
  { service: 'Payment',        port: 5006, method: 'GET',  endpoint: '/api/PaymentTypes/health',         description: 'Health check' },
  { service: 'Payment',        port: 5006, method: 'POST', endpoint: '/api/PaymentTypes/list',           description: 'List payment types' },
  { service: 'Payment',        port: 5006, method: 'GET',  endpoint: '/api/PaymentTypes/{id}',           description: 'Get payment type by ID' },
  { service: 'Payment',        port: 5006, method: 'POST', endpoint: '/api/PaymentTypes',                description: 'Create payment type' },
  { service: 'Payment',        port: 5006, method: 'PUT',  endpoint: '/api/PaymentTypes/{id}',           description: 'Update payment type' },
  { service: 'Payment',        port: 5006, method: 'GET',  endpoint: '/api/v1/management/payments',      description: 'Management - list payments' },

  // ── Promotion ────────────────────────────────────────────────
  { service: 'Promotion',      port: 5005, method: 'GET',  endpoint: '/api/Promotions/health',           description: 'Health check' },
  { service: 'Promotion',      port: 5005, method: 'POST', endpoint: '/api/Promotions/list',             description: 'List promotions' },
  { service: 'Promotion',      port: 5005, method: 'GET',  endpoint: '/api/Promotions/{id}',             description: 'Get promotion by ID' },
  { service: 'Promotion',      port: 5005, method: 'POST', endpoint: '/api/Promotions',                  description: 'Create promotion' },
  { service: 'Promotion',      port: 5005, method: 'PUT',  endpoint: '/api/Promotions/{id}',             description: 'Update promotion' },
  { service: 'Promotion',      port: 5005, method: 'GET',  endpoint: '/api/v1/management/promotions',    description: 'Management - list promotions' },

  // ── Report ───────────────────────────────────────────────────
  { service: 'Report',         port: null, method: 'GET',  endpoint: '/api/Reports/health',              description: 'Health check' },
  { service: 'Report',         port: null, method: 'POST', endpoint: '/api/Reports/list',                description: 'List reports' },
  { service: 'Report',         port: null, method: 'GET',  endpoint: '/api/Reports/{id}',                description: 'Get report by ID' },
  { service: 'Report',         port: null, method: 'POST', endpoint: '/api/Reports',                     description: 'Create report' },
  { service: 'Report',         port: null, method: 'PUT',  endpoint: '/api/Reports/{id}',                description: 'Update report' },

  // ── Organization ─────────────────────────────────────────────
  { service: 'Organization',   port: null, method: 'GET',  endpoint: '/api/Organizations/health',        description: 'Health check' },
];

// Color map per method
const methodColors = {
  GET:    'FF61AFFE',
  POST:   'FF49CC90',
  PUT:    'FFFCA130',
  DELETE: 'FFF93E3E',
  PATCH:  'FF50E3C2',
};

const serviceColors = {
  Authentication: 'FFDCE6F1',
  HR:             'FFE2EFDA',
  Customer:       'FFFFF2CC',
  Product:        'FFFCE4D6',
  Order:          'FFEDEDED',
  Payment:        'FFE8D5E8',
  Promotion:      'FFCCE5FF',
  Report:         'FFFFF9C4',
  Organization:   'FFF5F5F5',
};

const wb = XLSX.utils.book_new();
const wsData = [
  ['#', 'Service', 'Port', 'Method', 'Endpoint', 'Description'],
  ...apis.map((a, i) => [
    i + 1,
    a.service,
    a.port ?? '-',
    a.method,
    a.endpoint,
    a.description,
  ])
];

const ws = XLSX.utils.aoa_to_sheet(wsData);

// Column widths
ws['!cols'] = [
  { wch: 4 },   // #
  { wch: 16 },  // Service
  { wch: 7 },   // Port
  { wch: 9 },   // Method
  { wch: 52 },  // Endpoint
  { wch: 40 },  // Description
];

// Freeze header row
ws['!freeze'] = { xSplit: 0, ySplit: 1 };

// Style header
const headerStyle = {
  font: { bold: true, color: { rgb: 'FFFFFFFF' } },
  fill: { fgColor: { rgb: 'FF2F4F4F' } },
  alignment: { horizontal: 'center', vertical: 'center' },
  border: {
    top:    { style: 'thin', color: { rgb: 'FF000000' } },
    bottom: { style: 'thin', color: { rgb: 'FF000000' } },
    left:   { style: 'thin', color: { rgb: 'FF000000' } },
    right:  { style: 'thin', color: { rgb: 'FF000000' } },
  }
};

const cols = ['A','B','C','D','E','F'];
cols.forEach(c => {
  if (ws[`${c}1`]) ws[`${c}1`].s = headerStyle;
});

// Style data rows
apis.forEach((api, i) => {
  const row = i + 2;
  const svcColor = serviceColors[api.service] || 'FFFFFFFF';
  const methodColor = methodColors[api.method] || 'FFFFFFFF';

  cols.forEach(c => {
    const cell = ws[`${c}${row}`];
    if (!cell) return;
    const isMethod = c === 'D';
    cell.s = {
      fill: { fgColor: { rgb: isMethod ? methodColor : svcColor } },
      font: isMethod ? { bold: true } : {},
      alignment: { vertical: 'center', horizontal: isMethod ? 'center' : 'left' },
      border: {
        top:    { style: 'hair', color: { rgb: 'FFCCCCCC' } },
        bottom: { style: 'hair', color: { rgb: 'FFCCCCCC' } },
        left:   { style: 'hair', color: { rgb: 'FFCCCCCC' } },
        right:  { style: 'hair', color: { rgb: 'FFCCCCCC' } },
      }
    };
  });
});

XLSX.utils.book_append_sheet(wb, ws, 'API List');

const outPath = 'D:/PROJECTS/4_2026/01042026/API_List.xlsx';
XLSX.writeFile(wb, outPath);
console.log(`✅ Exported: ${outPath} (${apis.length} APIs)`);
