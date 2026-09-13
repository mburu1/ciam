import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import {
  AuthTokensResponse,
  LoginRequest,
  RegisterUserRequest,
  UserProfileResponse,
} from '../auth/models/auth.models';

@Injectable({ providedIn: 'root' })
export class ApiClient {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl.replace(/\/$/, '');

  login(request: LoginRequest) {
    return this.http.post<AuthTokensResponse>(`${this.baseUrl}/api/auth/login`, request);
  }

  register(request: RegisterUserRequest) {
    return this.http.post<UserProfileResponse>(
      `${this.baseUrl}/api/auth/register`,
      request,
    );
  }

  refresh(refreshToken: string) {
    return this.http.post<AuthTokensResponse>(`${this.baseUrl}/api/auth/refresh`, {
      refreshToken,
    });
  }

  currentUser() {
    return this.http.get<UserProfileResponse>(`${this.baseUrl}/api/users/me`);
  }

  updateProfile(request: Partial<UserProfileResponse>) {
    return this.http.put<UserProfileResponse>(
      `${this.baseUrl}/api/users/me`,
      request,
    );
  }
}
