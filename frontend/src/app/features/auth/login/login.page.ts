import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthShellComponent } from '../../../shared/ui/auth-shell/auth-shell.component';
import { AuthService } from '../../../core/auth/services/auth.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [AuthShellComponent, ReactiveFormsModule, RouterLink],
  selector: 'app-login-page',
  template: `
    <app-auth-shell>
      <div class="form-heading">
        <p class="eyebrow">Welcome back</p>
        <h2>Sign in to CIAM</h2>
        <p class="muted">Use your account credentials to continue.</p>
      </div>
      @if (error()) { <div class="alert" role="alert">{{ error() }}</div> }
      <form [formGroup]="form" (ngSubmit)="submit()" novalidate>
        <label for="username">Username or email</label>
        <input id="username" formControlName="usernameOrEmail" autocomplete="username" />
        @if (form.controls.usernameOrEmail.touched && form.controls.usernameOrEmail.invalid) {
          <small class="field-error">Enter your username or email.</small>
        }
        <label for="password">Password</label>
        <input id="password" type="password" formControlName="password" autocomplete="current-password" />
        @if (form.controls.password.touched && form.controls.password.invalid) {
          <small class="field-error">Password must contain at least 8 characters.</small>
        }
        <label class="checkbox"><input type="checkbox" formControlName="rememberMe" /> <span>Keep me signed in</span></label>
        <button class="button primary" type="submit" [disabled]="form.invalid || auth.isLoading()">
          {{ auth.isLoading() ? 'Signing in…' : 'Sign in' }}
        </button>
      </form>
      <p class="form-footer">New to CIAM? <a routerLink="/auth/register">Create an account</a></p>
    </app-auth-shell>
  `,
  styles: `
    .form-heading { margin-bottom: 2rem; } h2 { margin: .45rem 0 .55rem; font-size: 2rem; letter-spacing: -.04em; } .muted { color: var(--text-muted); }
    .eyebrow { color: var(--accent-strong); font-size: .75rem; font-weight: 800; letter-spacing: .14em; text-transform: uppercase; }
    form { display: grid; gap: .55rem; } label { margin-top: .7rem; font-size: .86rem; font-weight: 700; } input:not([type=checkbox]) { width: 100%; padding: .85rem 1rem; border: 1px solid var(--border); border-radius: .7rem; background: #fff; color: var(--text); } input:focus { outline: 3px solid #14b8a633; border-color: var(--accent); }
    .checkbox { display: flex; align-items: center; gap: .5rem; margin: .6rem 0 1rem; font-weight: 500; } .checkbox input { accent-color: var(--accent); }
    .button { border: 0; border-radius: .7rem; padding: .9rem 1rem; font: inherit; font-weight: 800; cursor: pointer; } .primary { color: #fff; background: var(--accent-strong); } .button:disabled { cursor: not-allowed; opacity: .55; }
    .alert { margin-bottom: 1rem; padding: .8rem 1rem; border-radius: .6rem; color: #991b1b; background: #fee2e2; font-size: .88rem; } .field-error { color: #b91c1c; } .form-footer { margin-top: 1.5rem; text-align: center; color: var(--text-muted); font-size: .9rem; } a { color: var(--accent-strong); font-weight: 700; }
  `,
})
export class LoginPage {
  readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  readonly error = signal('');
  readonly form = this.fb.nonNullable.group({
    usernameOrEmail: ['', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    rememberMe: [true],
  });

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.error.set('');
    this.auth.login(this.form.getRawValue()).subscribe({
      next: () => this.router.navigateByUrl('/dashboard'),
      error: (error: { error?: { message?: string } }) =>
        this.error.set(error.error?.message ?? 'Unable to sign in. Check your credentials and try again.'),
    });
  }
}
