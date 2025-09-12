import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Unit } from '../../models';
import { ApiService } from '../../core/api';

@Component({
  selector: 'app-units',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './units.html',
  styleUrl: './units.scss'
})
export class UnitsComponent implements OnInit {
  units: Unit[] = [];
  form: Partial<Unit> = { status: 'Vacant' };

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.api.listUnits().subscribe({
      next: (res) => (this.units = res),
      error: () => (this.units = []),
    });
  }

  save() {
    if (this.form.id) {
      this.api.updateUnit(this.form.id, this.form).subscribe(() => this.load());
    } else {
      const { propertyId, unitNumber } = this.form;
      if (!propertyId || !unitNumber) return;
      this.api.createUnit(this.form as Omit<Unit, 'id'>).subscribe(() => {
        this.form = { status: 'Vacant' };
        this.load();
      });
    }
  }
  edit(u: Unit) {
  this.form = { ...u };
}
}
