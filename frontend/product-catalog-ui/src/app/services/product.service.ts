import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Product,
  ProductSummary,
  CreateProductRequest,
  UpdateProductRequest,
} from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly base = '/api/products';

  getProducts(search?: string, categoryId?: string): Observable<ProductSummary[]> {
    const params: Record<string, string> = {};
    if (search) params['search'] = search;
    if (categoryId) params['categoryId'] = categoryId;
    return this.http.get<ProductSummary[]>(this.base, { params });
  }

  getById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.base}/${id}`);
  }

  create(request: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(this.base, request);
  }

  update(id: string, request: UpdateProductRequest): Observable<Product> {
    return this.http.put<Product>(`${this.base}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  adjustStock(id: string, delta: number): Observable<void> {
    return this.http.patch<void>(`${this.base}/${id}/stock`, { delta });
  }

  discontinue(id: string): Observable<void> {
    return this.http.post<void>(`${this.base}/${id}/discontinue`, {});
  }
}
