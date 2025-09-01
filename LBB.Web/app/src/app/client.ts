import { ClientOptions } from './api';
import { environment } from '../environments/environment';

export const clientOptions: ClientOptions = {
  baseUrl: environment.apiUrl,
};
