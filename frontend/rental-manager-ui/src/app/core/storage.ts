import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class StorageService {
  private get hasBrowser() {
    return typeof window !== 'undefined' && !!window.localStorage;
  }
  getItem(key: string): string | null { return this.hasBrowser ? localStorage.getItem(key) : null; }
  setItem(key: string, value: string): void { if (this.hasBrowser) localStorage.setItem(key, value); }
  removeItem(key: string): void { if (this.hasBrowser) localStorage.removeItem(key); }
  clear(): void { if (this.hasBrowser) localStorage.clear(); }
}
