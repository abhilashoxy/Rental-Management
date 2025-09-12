import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Property, Unit, Tenant } from '../models'; // barrel import
import { environment } from '../../environments/environments';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private base = environment.apiBaseUrl;
  constructor(private http: HttpClient) {}

  // Dashboard
  getSummary() {
    return this.http.get<{
      unitsTotal: number;
      unitsOccupied: number;
      occupancyPercent: number;
      dueThisMonth: number;
      overdueLeases: number;
    }>(`${this.base}/api/dashboard/summary`);
  }

  // Properties
  listProperties() {
    return this.http.get<Property[]>(`${this.base}/api/properties`);
  }
  createProperty(p: Omit<Property, 'id' | 'createdAt' | 'units'>) {
    return this.http.post<Property>(`${this.base}/api/properties`, p);
  }

  // Units
  listUnits(propertyId?: number) {
    const q = propertyId ? `?propertyId=${propertyId}` : '';
    return this.http.get<Unit[]>(`${this.base}/api/units${q}`);
  }
  createUnit(u: Omit<Unit, 'id'>) {
    return this.http.post<Unit>(`${this.base}/api/units`, u);
  }
  updateUnit(id: number, u: Partial<Unit>) {
    return this.http.patch<Unit>(`${this.base}/api/units/${id}`, u);
  }

  // Tenants
  listTenants() {
    return this.http.get<Tenant[]>(`${this.base}/api/tenants`);
  }
  createTenant(t: Omit<Tenant, 'id'>) {
    return this.http.post<Tenant>(`${this.base}/api/tenants`, t);
  }
}
