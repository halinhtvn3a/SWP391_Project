const { $ } = await import('bun');
const url = process.env.OPENAPI_URL;
if (!url) throw new Error('Missing OPENAPI_URL in .env');

await $`openapi-typescript ${url} --output src/api/types.ts`;
