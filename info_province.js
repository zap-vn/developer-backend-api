const { Client } = require('pg');
const client = new Client({ host: '136.118.121.105', port: 5432, user: 'postgres', password: 'Pg@Secret2026!', database: 'zap_ecosystem_v110' });
client.connect().then(async () => {
    const res = await client.query("SELECT column_name, data_type FROM information_schema.columns WHERE table_schema='system' AND table_name='geo_province';");
    console.log('geo_province:');
    console.log(res.rows.map(r => r.column_name + ' ' + r.data_type).join('\n'));

    const res2 = await client.query("SELECT column_name, data_type FROM information_schema.columns WHERE table_schema='system' AND table_name='geo_province_translation';");
    console.log('\ngeo_province_translation:');
    console.log(res2.rows.map(r => r.column_name + ' ' + r.data_type).join('\n'));
    client.end();
}).catch(console.error);
