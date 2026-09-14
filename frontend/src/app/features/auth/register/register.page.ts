import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthShellComponent } from '../../../shared/ui/auth-shell/auth-shell.component';
import { AuthService } from '../../../core/auth/services/auth.service';
import { FrontendLogger } from '../../../core/logging/frontend-logger.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [AuthShellComponent, ReactiveFormsModule, RouterLink],
  selector: 'app-register-page',
  template: `
    <app-auth-shell>
      <div class="form-heading"><p class="eyebrow">Get started</p><h2>Create your account</h2><p class="muted">Set up your secure customer identity.</p></div>
      @if (error()) { <div class="alert" role="alert">{{ error() }}</div> }
      @if (success()) { <div class="success" role="status">{{ success() }}</div> }
      <form [formGroup]="form" (ngSubmit)="submit()" novalidate>
        <div class="name-grid"><div><label for="firstName">First name</label><input id="firstName" formControlName="firstName" autocomplete="given-name" /></div><div><label for="lastName">Last name</label><input id="lastName" formControlName="lastName" autocomplete="family-name" /></div></div>
        <label for="email">Email</label><input id="email" type="email" formControlName="email" autocomplete="email" />
        <label for="username">Preferred username</label><input id="username" formControlName="preferredUsername" autocomplete="username" />
        <label for="phone">Phone <span class="optional">optional</span></label><input id="phone" type="tel" formControlName="phoneNumber" autocomplete="tel" />
        <label for="locale">Locale <span class="optional">optional</span></label><input id="locale" formControlName="locale" placeholder="en-KE" />
        <button class="button primary" type="submit" [disabled]="form.invalid || auth.isLoading()">{{ auth.isLoading() ? 'Creating account…' : 'Create account' }}</button>
      </form>
      <p class="form-footer">Already have an account? <a routerLink="/auth/login">Sign in</a></p>
    </app-auth-shell>
  `,
  styles: `
    .form-heading { margin-bottom: 1.5rem; } h2 { margin: .45rem 0 .55rem; font-size: 2rem; letter-spacing: -.04em; } .muted { color: var(--text-muted); } .eyebrow { color: var(--accent-strong); font-size: .75rem; font-weight: 800; letter-spacing: .14em; text-transform: uppercase; }
    form { display: grid; gap: .55rem; } .name-grid { display: grid; grid-template-columns: 1fr 1fr; gap: .8rem; } label { margin-top: .55rem; font-size: .82rem; font-weight: 700; } .optional { color: var(--text-muted); font-weight: 500; } input { box-sizing: border-box; width: 100%; margin-top: .35rem; padding: .78rem .9rem; border: 1px solid var(--border); border-radius: .7rem; background: #fff; color: var(--text); } input:focus { outline: 3px solid #14b8a633; border-color: var(--accent); }
    .button { margin-top: .8rem; border: 0; border-radius: .7rem; padding: .9rem 1rem; font: inherit; font-weight: 800; cursor: pointer; } .primary { color: #fff; background: var(--accent-strong); } .button:disabled { cursor: not-allowed; opacity: .55; } .alert, .success { margin-bottom: 1rem; padding: .8rem 1rem; border-radius: .6rem; font-size: .88rem; } .alert { color: #991b1b; background: #fee2e2; } .success { color: #166534; background: #dcfce7; } .form-footer { margin-top: 1.3rem; text-align: center; color: var(--text-muted); font-size: .9rem; } a { color: var(--accent-strong); font-weight: 700; } @media (max-width: 460px) { .name-grid { grid-template-columns: 1fr; gap: 0; } }
  `,
})
export class RegisterPage {
  readonly auth = inject(AuthService);
  private readonly logger = inject(FrontendLogger);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  readonly error = signal('');
  readonly success = signal('');
  readonly form = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email]],
    preferredUsername: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(64)]],
    phoneNumber: [''],
    locale: [''],
  });

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.error.set('');
    this.success.set('');
    this.auth.register(this.form.getRawValue()).subscribe({
      next: () => {
        this.success.set('Your account was created. You can now sign in.');
        setTimeout(() => this.router.navigateByUrl('/auth/login'), 1200);
      },
      error: (error: { error?: { message?: string; errorCode?: string }; status?: number; statusText?: string }) => {
        this.logger.error('Account registration failed', {
          method: 'POST',
          url: '/api/auth/register',
          status: error.status,
          statusText: error.statusText,
          errorCode: error.error?.errorCode,
        }, error);
        this.error.set(error.error?.message ?? 'Unable to create your account. Please try again.');
      },
    });
  }
}
