import { client } from './client';
import { useMutation, useQuery } from '@tanstack/react-query';
import {
  type HttpMethod,
  type PathsWithMethod,
} from 'openapi-typescript-helpers';
import { type FetchOptions } from 'openapi-fetch';
import type { paths } from './types'; // Example of importing paths from types (will depend on BE OpenAPI spec)
import { type UseMutationOptions as RQUseMutationOptions } from '@tanstack/react-query';
import { type UseQueryOptions as RQUseQueryOptions } from '@tanstack/react-query';

// Define types for paths and parameters
type Paths<M extends HttpMethod> = PathsWithMethod<paths, M>;
type Params<M extends HttpMethod, P extends Paths<M>> = M extends keyof paths[P]
  ? FetchOptions<paths[P][M]>
  : never;

// Define types for query and mutation options
type UseQueryOptions = Pick<RQUseQueryOptions, 'enabled'>;
type UseMutationOptions = Pick<RQUseMutationOptions, 'retry'>;

/**
 * @param path - The API endpoint path to query.
 * @param params - The parameters to be sent with the request.
 * @description
 * This function is used to perform a GET request using the OpenAPI client.
 * @returns
 * This function is used to perform a GET request using the OpenAPI client.
 * It returns the result of the query.
 * @example
 * const gists = useGetQuery('/gists', { params: { query: { per_page: 5 } } })
 * gists.data?.map((gist) => (...))
 * gists.refetch()
 */
export function useGetQuery<P extends Paths<'get'>>(
  path: P,
  params: Params<'get', P> & { rq?: UseQueryOptions }
) {
  const { rq, ...queryParams } = params;
  return useQuery<any, Error, any, readonly unknown[]>({
    queryKey: [path, JSON.stringify(queryParams)],
    queryFn: async () => {
      const { data, error } = await client.GET(path, params);
      if (error) throw error;
      return { data, error: null };
    },
    ...params?.rq,
  });
}

/**
 * @param path - The API endpoint path to mutate.
 * @param options - The options to configure the mutation.
 * @description
 * This function is used to perform a POST request using the OpenAPI client.
 * @returns
 * This function is used to perform a POST request using the OpenAPI client.
 * It returns a mutation object that can be used to trigger the request.
 * @example
 *   const createGist = usePostMutation('/gists')
 * createGist.mutate(
 *   {
 *     body: {
 *       description: new Date().toISOString(),
 *       files: { 'greeting.txt': { content: 'hello, world' } },
 *     },
 *   },
 *   { onSuccess }
 * )
 */
export function usePostMutation<P extends Paths<'post'>>(
  path: P,
  options?: UseMutationOptions
) {
  return useMutation({
    mutationFn: async (params: Params<'post', P>) => {
      const { data, error } = await client.POST(path, params);
      if (error) throw error;
      return data;
    },
    ...options,
  });
}

/**
 *
 * @param path - The API endpoint path to mutate.
 * @param options - The options to configure the mutation.
 * @description
 * This function is used to perform a PUT request using the OpenAPI client.
 * @returns
 * This function is used to perform a PUT request using the OpenAPI client.
 * It returns a mutation object that can be used to trigger the request.
 * @example
 * const updateGist = usePutMutation('/gists/{gist_id}');
 * updateGist.mutate({
 *   params: {
 *     path: { gist_id: '123' },
 *     body: {
 *       description: new Date().toISOString(),
 *       files: { 'greeting.txt': { content: 'hello, world' } },
 *     },
 *   },
 * });
 */
export function usePutMutation<P extends Paths<'put'>>(
  path: P,
  options?: UseMutationOptions
) {
  return useMutation({
    mutationFn: async (params: Params<'put', P>) => {
      const { data, error } = await client.PUT(path, params);
      if (error) throw error;
      return data;
    },
    ...options,
  });
}

/**
 *
 * @param path - The API endpoint path to mutate.
 * @param options - The options to configure the mutation.
 * @description
 * This function is used to perform a DELETE request using the OpenAPI client.
 * @returns
 * This function is used to perform a DELETE request using the OpenAPI client.
 * It returns a mutation object that can be used to trigger the request.
 * @example
 * const removeGist = useDeleteMutation('/gists/{gist_id}')
 * removeGist.mutate({ params: { path: { gist_id: id } } }, { onSuccess })
 */
export function useDeleteMutation<P extends Paths<'delete'>>(
  path: P,
  options?: UseMutationOptions
) {
  return useMutation({
    mutationFn: async (params: Params<'delete', P>) => {
      const { data, error } = await client.DELETE(path, params);
      if (error) throw error;
      return data;
    },
    ...options,
  });
}
