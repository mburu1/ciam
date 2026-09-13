export interface LoginRequest {
  usernameOrEmail: string;
  password: string;
  deviceName?: string;
  rememberMe: boolean;
}

export interface RegisterUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  preferredUsername: string;
  phoneNumber?: string;
  locale?: string;
}

export interface AuthTokensResponse {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAtUtc: string;
  refreshTokenExpiresAtUtc: string;
  tokenType: string;
}

export interface UserProfileResponse {
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  preferredUsername: string;
  phoneNumber?: string;
  locale?: string;
  emailVerified: boolean;
  status: string;
  createdAtUtc: string;
  updatedAtUtc: string;
}
