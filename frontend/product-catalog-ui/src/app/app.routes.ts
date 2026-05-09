import { Routes } from '@angular/router';
import { ProductListComponent } from './components/product-list/product-list.component';
import { ProductDetailComponent } from './components/product-detail/product-detail.component';
import { ProductFormComponent } from './components/product-form/product-form.component';
import { CategoryListComponent } from './components/category-list/category-list.component';

export const routes: Routes = [
  { path: '', redirectTo: 'products', pathMatch: 'full' },
  { path: 'products', component: ProductListComponent },
  { path: 'products/new', component: ProductFormComponent },
  { path: 'products/:id', component: ProductDetailComponent },
  { path: 'products/:id/edit', component: ProductFormComponent },
  { path: 'categories', component: CategoryListComponent },
  { path: '**', redirectTo: 'products' },
];
