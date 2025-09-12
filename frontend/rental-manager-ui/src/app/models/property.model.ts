export interface Property {
  id: number;
  name: string;
  address: string;
  city: string;
  state: string;
  zip: string;
  createdAt: string; // ISO
  units?: Unit[];    // optional to avoid circular fetch issues
}

export interface Unit {
  id: number;
  propertyId: number;
  unitNumber: string;
  bedrooms: number;
  bathrooms: number;
  rentAmount: number;
  status: 'Vacant' | 'Occupied' | 'Unavailable';
}
