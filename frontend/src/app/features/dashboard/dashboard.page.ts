import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/services/auth.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink],
  selector: 'app-dashboard-page',
  template: `
    <header class="topbar"><a class="brand" routerLink="/"><span class="brand-mark">C</span><span>CIAM</span></a><button class="logout" type="button" (click)="logout()">Sign out</button></header>
    <main class="dashboard"><section class="welcome"><p class="eyebrow">Your account</p><h1>Good to see you<span>.</span></h1><p>Manage your identity and keep your account secure.</p></section>
      <section class="grid"><article class="profile card"><div class="card-title"><h2>Profile</h2><span class="badge">Active</span></div>@if (auth.user(); as user) {<div class="profile-main"><div class="avatar">{{ user.firstName.charAt(0) }}{{ user.lastName.charAt(0) }}</div><div><h3>{{ user.firstName }} {{ user.lastName }}</h3><p>{{ user.email }}</p></div></div><dl><div><dt>Username</dt><dd>{{ user.preferredUsername }}</dd></div><div><dt>Email verification</dt><dd>{{ user.emailVerified ? 'Verified' : 'Pending' }}</dd></div></dl>} @else {<p class="muted">Loading profile…</p>}</article>
        <article class="card security"><div class="card-title"><h2>Security</h2><span class="secure">● Protected</span></div><p class="muted">Your account is protected by Keycloak identity controls.</p><div class="security-item"><span class="security-icon">✓</span><span><strong>Authenticated session</strong><small>Access token is active</small></span></div><div class="security-item"><span class="security-icon">+</span><span><strong>Passkeys</strong><small>Set up phishing-resistant access soon</small></span></div></article>
      </section>
    </main>
  `,
  styles: `
    :host { display: block; min-height: 100dvh; background: var(--surface-0); } .topbar { display: flex; justify-content: space-between; align-items: center; padding: 1.4rem 2rem; border-bottom: 1px solid var(--border); background: #fff; } .brand { display: inline-flex; align-items: center; gap: .7rem; color: var(--text); font-weight: 850; letter-spacing: .15em; text-decoration: none; } .brand-mark { display: grid; width: 2.3rem; height: 2.3rem; place-items: center; border-radius: .7rem; color: #fff; background: var(--accent-strong); letter-spacing: 0; } .logout { border: 0; color: var(--accent-strong); background: transparent; font: inherit; font-size: .88rem; font-weight: 800; cursor: pointer; } .dashboard { max-width: 70rem; margin: auto; padding: 5rem 2rem; } .eyebrow { color: var(--accent-strong); font-size: .75rem; font-weight: 850; letter-spacing: .15em; text-transform: uppercase; } h1 { margin: .6rem 0 .7rem; font-size: clamp(2.6rem, 6vw, 4.5rem); letter-spacing: -.07em; } h1 span { color: var(--accent-strong); } .welcome > p:last-child { color: var(--text-muted); font-size: 1.1rem; } .grid { display: grid; grid-template-columns: 1.1fr .9fr; gap: 1.2rem; margin-top: 3rem; } .card { padding: 1.5rem; border: 1px solid var(--border); border-radius: 1rem; background: #fff; box-shadow: 0 .8rem 2rem #0f172a08; } .card-title { display: flex; align-items: center; justify-content: space-between; gap: 1rem; } h2 { margin: 0; font-size: 1.1rem; } .badge, .secure { padding: .35rem .6rem; border-radius: 999px; color: #166534; background: #dcfce7; font-size: .72rem; font-weight: 800; } .secure { color: var(--accent-strong); background: #ccfbf1; } .profile-main { display: flex; align-items: center; gap: 1rem; margin: 2rem 0; } .avatar { display: grid; width: 3.6rem; height: 3.6rem; place-items: center; border-radius: 1rem; color: #fff; background: var(--accent-strong); font-size: 1.1rem; font-weight: 850; } h3 { margin: 0 0 .25rem; } .profile-main p, .muted { margin: 0; color: var(--text-muted); } dl { display: grid; gap: .9rem; margin: 0; padding-top: 1rem; border-top: 1px solid var(--border); } dl div { display: flex; justify-content: space-between; gap: 1rem; } dt { color: var(--text-muted); font-size: .85rem; } dd { margin: 0; font-size: .85rem; font-weight: 700; } .security { display: grid; align-content: start; gap: 1.2rem; } .security > p { margin: .5rem 0; line-height: 1.5; } .security-item { display: flex; align-items: center; gap: .8rem; padding-top: 1rem; border-top: 1px solid var(--border); } .security-icon { display: grid; width: 2rem; height: 2rem; place-items: center; border-radius: .55rem; color: #166534; background: #dcfce7; font-weight: 900; } .security-item span:nth-child(2) { display: grid; gap: .2rem; } .security-item small { color: var(--text-muted); } @media (max-width: 700px) { .grid { grid-template-columns: 1fr; } .dashboard { padding-top: 3rem; } }
  `,
})
export class DashboardPage {
  readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  constructor() {
    this.auth.loadCurrentUser().subscribe();
  }

  logout() {
    this.auth.logout();
    void this.router.navigateByUrl('/');
  }
}
