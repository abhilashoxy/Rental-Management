export interface Payment {
  id: number;
  leaseId: number;
  paymentDate: string;  // ISO
  amount: number;
  method: 'Cash' | 'UPI' | 'Card' | 'Bank';
  notes?: string;
}
