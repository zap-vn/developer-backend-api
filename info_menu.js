const { Client } = require('pg');
const client = new Client({ host: '136.118.121.105', port: 5432, user: 'postgres', password: 'Pg@Secret2026!', database: 'zap_ecosystem_v110' });
client.connect().then(async () => {
    const res = await client.query("SELECT column_name, data_type FROM information_schema.columns WHERE table_schema='catalog' AND table_name='menu_header';");
    console.log(res.rows.map(r => r.column_name + ' ' + r.data_type).join('\n'));
    client.end();
}).catch(console.error);
