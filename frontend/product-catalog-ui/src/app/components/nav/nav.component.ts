import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  template: `
    <nav class="bg-indigo-700 text-white shadow">
      <div class="max-w-7xl mx-auto px-4 flex items-center h-14 gap-6">
        <span class="font-bold text-lg tracking-tight">Product Catalog</span>
        <a
          routerLink="/products"
          routerLinkActive="underline font-semibold"
          class="hover:text-indigo-200 transition-colors">
          Products
        </a>
        <a
          routerLink="/categories"
          routerLinkActive="underline font-semibold"
          class="hover:text-indigo-200 transition-colors">
          Categories
        </a>
      </div>
    </nav>
  `,
})
export class NavComponent {}
