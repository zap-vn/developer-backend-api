const { Client } = require('pg');
const client = new Client({ host: '136.118.121.105', port: 5432, user: 'postgres', password: 'Pg@Secret2026!', database: 'zap_ecosystem_v110' });
client.connect().then(async () => {
    const res = await client.query("SELECT table_schema, table_name FROM information_schema.tables WHERE table_name LIKE '%promotion%';");
    console.log(res.rows.map(r => r.table_schema + '.' + r.table_name).join('\n'));
    client.end();
}).catch(console.error);
