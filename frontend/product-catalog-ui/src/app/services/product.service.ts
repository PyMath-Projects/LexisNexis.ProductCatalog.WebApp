import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import {
  Product,
  ProductSummary,
  CreateProductRequest,
  UpdateProductRequest,
} from '../models/product.model';

export interface SearchResult {
  products: ProductSummary[];
  cacheHit: boolean;
}

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly base = '/api/products';

  getProducts(categoryId?: string): Observable<ProductSummary[]> {
    const params: Record<string, string> = {};
    if (categoryId) params['categoryId'] = categoryId;
    return this.http.get<ProductSummary[]>(this.base, { params });
  }

  search(query: string): Observable<SearchResult> {
    return this.http
      .get<ProductSummary[]>(this.base, {
        params: { search: query },
        observe: 'response',
      })
      .pipe(
        map((response) => ({
          products: response.body ?? [],
          cacheHit: response.headers.get('X-Cache') === 'HIT',
        }))
      );
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
