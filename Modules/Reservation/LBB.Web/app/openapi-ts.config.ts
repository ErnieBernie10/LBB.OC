import { defaultPlugins, defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
  input: 'https://localhost:7047/openapi/v1.json',
  output: 'src/app/api',
  plugins: [
    { name: '@hey-api/schemas', type: 'json' },
    {
      name: '@hey-api/typescript',
    },
  ],
});
