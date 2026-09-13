import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink],
  selector: 'app-auth-shell',
  template: `
    <main class="auth-shell">
      <section class="auth-brand">
        <a class="brand" routerLink="/">
          <span class="brand-mark">C</span>
          <span>CIAM</span>
        </a>
        <div class="brand-copy">
          <p class="eyebrow">Secure identity</p>
          <h1>Access your world with confidence.</h1>
          <p>One secure identity for every customer experience.</p>
        </div>
        <div class="trust-note">
          <span class="status-dot"></span>
          <span>Protected by enterprise-grade identity controls</span>
        </div>
      </section>
      <section class="auth-panel">
        <div class="auth-card">
          <ng-content />
        </div>
      </section>
    </main>
  `,
  styles: `
    :host { display: block; min-height: 100dvh; }
    .auth-shell { min-height: 100dvh; display: grid; grid-template-columns: 1fr 1fr; background: var(--surface-0); }
    .auth-brand { padding: clamp(2rem, 7vw, 7rem); display: flex; flex-direction: column; justify-content: space-between; color: #fff; background: linear-gradient(145deg, #12233f, #1c4e5f 58%, #167a7a); }
    .brand { display: inline-flex; align-items: center; gap: .75rem; color: #fff; font-weight: 800; letter-spacing: .16em; text-decoration: none; }
    .brand-mark { display: grid; width: 2.5rem; height: 2.5rem; place-items: center; border-radius: .75rem; color: #12233f; background: #a7f3d0; letter-spacing: 0; }
    .brand-copy { max-width: 32rem; }
    .brand-copy h1 { margin: .8rem 0 1.2rem; font-size: clamp(2.4rem, 5vw, 4.6rem); line-height: .98; letter-spacing: -.06em; }
    .brand-copy p:not(.eyebrow) { color: #c9e2e4; font-size: 1.1rem; }
    .eyebrow { color: #a7f3d0; font-size: .75rem; font-weight: 800; letter-spacing: .16em; text-transform: uppercase; }
    .trust-note { display: flex; align-items: center; gap: .6rem; color: #c9e2e4; font-size: .82rem; }
    .status-dot { width: .55rem; height: .55rem; border-radius: 50%; background: #a7f3d0; box-shadow: 0 0 0 .3rem #a7f3d022; }
    .auth-panel { display: grid; place-items: center; padding: 2rem; }
    .auth-card { width: min(100%, 29rem); }
    @media (max-width: 800px) { .auth-shell { grid-template-columns: 1fr; } .auth-brand { min-height: 16rem; padding: 2rem; gap: 3rem; } .trust-note { display: none; } }
  `,
})
export class AuthShellComponent {
  readonly title = input('');
}
