import { InjectionToken } from '@angular/core';

/** Base URL of the ASP.NET Core API. Adjust here if the backend port changes. */
export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL', {
  providedIn: 'root',
  factory: () => 'http://localhost:5074/api',
});
