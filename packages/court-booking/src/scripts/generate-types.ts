// @ts-ignore
const { $ } = await import('bun')
const url = import.meta.env.VITE_OPENAPI_URL
if (!url) throw new Error('Missing VITE_OPENAPI_URL in .env')

await $`openapi-typescript ${url} --output src/api/types.ts`
export {}
