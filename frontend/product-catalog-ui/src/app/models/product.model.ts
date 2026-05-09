export interface ProductSummary {
  id: string;
  name: string;
  sku: string;
  price: number;
  currency: string;
  quantity: number;
  status: string;
  categoryId: string;
}

export interface Product {
  id: string;
  name: string;
  description: string | null;
  sku: string;
  price: number;
  currency: string;
  quantity: number;
  categoryId: string;
  status: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProductRequest {
  name: string;
  description: string | null;
  sku: string;
  price: number;
  currency: string;
  initialQuantity: number;
  categoryId: string;
}

export interface UpdateProductRequest {
  name: string;
  description: string | null;
  price: number;
  currency: string;
  categoryId: string;
}
