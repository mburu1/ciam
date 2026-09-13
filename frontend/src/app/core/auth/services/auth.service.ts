import { Injectable, computed, inject, signal } from '@angular/core';
import { catchError, finalize, map, of, switchMap, tap } from 'rxjs';
import { ApiClient } from '../../api/api-client.service';
import {
  AuthTokensResponse,
  LoginRequest,
  RegisterUserRequest,
  UserProfileResponse,
} from '../models/auth.models';

const tokenStorageKey = 'ciam.auth.tokens';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly api = inject(ApiClient);
  private readonly tokensState = signal<AuthTokensResponse | null>(this.readTokens());
  private readonly userState = signal<UserProfileResponse | null>(null);
  private readonly loadingState = signal(false);

  readonly tokens = this.tokensState.asReadonly();
  readonly user = this.userState.asReadonly();
  readonly isLoading = this.loadingState.asReadonly();
  readonly isAuthenticated = computed(() => {
    const tokens = this.tokensState();
    return Boolean(tokens?.accessToken && !this.isExpired(tokens.accessTokenExpiresAtUtc));
  });

  login(request: LoginRequest) {
    this.loadingState.set(true);
    return this.api.login(request).pipe(
      tap((tokens) => this.storeTokens(tokens)),
      switchMap(() => this.loadCurrentUser()),
      finalize(() => this.loadingState.set(false)),
    );
  }

  register(request: RegisterUserRequest) {
    this.loadingState.set(true);
    return this.api.register(request).pipe(
      finalize(() => this.loadingState.set(false)),
    );
  }

  loadCurrentUser() {
    if (!this.isAuthenticated()) {
      return of(null);
    }

    return this.api.currentUser().pipe(
      tap((user) => this.userState.set(user)),
      catchError(() => {
        this.clearSession();
        return of(null);
      }),
    );
  }

  refresh() {
    const refreshToken = this.tokensState()?.refreshToken;
    if (!refreshToken) {
      return of(false);
    }

    return this.api.refresh(refreshToken).pipe(
      tap((tokens) => this.storeTokens(tokens)),
      map(() => true),
      catchError(() => {
        this.clearSession();
        return of(false);
      }),
    );
  }

  logout() {
    this.clearSession();
  }

  accessToken(): string | null {
    return this.isAuthenticated() ? this.tokensState()?.accessToken ?? null : null;
  }

  private storeTokens(tokens: AuthTokensResponse) {
    this.tokensState.set(tokens);
    localStorage.setItem(tokenStorageKey, JSON.stringify(tokens));
  }

  private clearSession() {
    this.tokensState.set(null);
    this.userState.set(null);
    localStorage.removeItem(tokenStorageKey);
  }

  private readTokens(): AuthTokensResponse | null {
    try {
      const raw = localStorage.getItem(tokenStorageKey);
      return raw ? (JSON.parse(raw) as AuthTokensResponse) : null;
    } catch {
      localStorage.removeItem(tokenStorageKey);
      return null;
    }
  }

  private isExpired(value: string) {
    return !value || Date.parse(value) <= Date.now();
  }
}
