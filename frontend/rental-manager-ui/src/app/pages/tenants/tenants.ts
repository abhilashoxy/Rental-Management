import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Tenant } from '../../models';
import { ApiService } from '../../core/api';

@Component({
  selector: 'app-tenants',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tenants.html',
  styleUrl: './tenants.scss'
})
export class TenantsComponent implements OnInit {
  tenants: Tenant[] = [];
  form: Partial<Tenant> = {};

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.api.listTenants().subscribe({
      next: (res) => (this.tenants = res),
      error: () => (this.tenants = []),
    });
  }

  create() {
    if (!this.form.firstName) return;
    this.api.createTenant(this.form as Omit<Tenant, 'id'>).subscribe(() => {
      this.form = {};
      this.load();
    });
  }
  edit(t: Tenant) {
  this.form = { ...t }; // safe in TS, not in template
}

}
