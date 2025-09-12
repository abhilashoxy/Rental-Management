export interface Lease {
  id: number;
  unitId: number;
  tenantId: number;
  startDate: string;    // ISO
  endDate: string;      // ISO
  monthlyRent: number;
  deposit: number;
  status: 'Active' | 'Terminated' | 'Expired';
}
