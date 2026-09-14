import { Injectable } from '@angular/core';

export interface FrontendLogContext {
  method?: string;
  url?: string;
  status?: number;
  statusText?: string;
  errorCode?: string;
  requestId?: string;
  [key: string]: string | number | undefined;
}

@Injectable({ providedIn: 'root' })
export class FrontendLogger {
  error(message: string, context: FrontendLogContext = {}, error?: unknown): void {
    const details = this.extractErrorDetails(error);
    console.error(`[CIAM] ${message}`, {
      ...context,
      ...details,
      occurredAtUtc: new Date().toISOString(),
    });
  }

  private extractErrorDetails(error: unknown): FrontendLogContext {
    if (!error || typeof error !== 'object') {
      return {};
    }

    const candidate = error as {
      error?: { errorCode?: string; message?: string; details?: string };
      message?: string;
    };
    const apiError = candidate.error;

    return {
      errorMessage: apiError?.message ?? candidate.message,
      errorDetails: apiError?.details,
      errorCode: apiError?.errorCode,
    };
  }
}
