import createClient from 'openapi-fetch';
import type { Middleware } from 'openapi-fetch';
import type { paths } from './types.ts'; // Example of importing paths from types (will be replace by BE OpenAPI spec)

const githubToken = import.meta.env.VITE_GITHUB_TOKEN;
const baseUrl = 'https://api.github.com';

const authMiddleware: Middleware = {
  onRequest: async ({ request }) => {
    request.headers.set('Authorization', `Bearer ${githubToken}`);
    return request;
  },
};

const client = createClient<paths>({ baseUrl });

client.use(authMiddleware);

const path: keyof paths = '/advisories';
client.GET(path);

export { client };
