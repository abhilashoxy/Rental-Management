import { Component, OnInit } from '@angular/core';
import { Property } from '../../models';
import { ApiService } from '../../core/api';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-properties',
 standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './properties.html',
  styleUrl: './properties.scss'
})
export class PropertiesComponent implements OnInit {
  properties: Property[] = [];
  form: Partial<Property> = { name: '', address: '', city: '', state: '', zip: '' };

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.api.listProperties().subscribe({
      next: (res) => (this.properties = res),
      error: () => (this.properties = []),
    });
  }

  create() {
    const { name, address, city, state, zip } = this.form;
    if (!name) return;
    this.api
      .createProperty({
        name: name!,
        address: address ?? '',
        city: city ?? '',
        state: state ?? '',
        zip: zip ?? '',
      })
      .subscribe(() => {
        this.form = { name: '', address: '', city: '', state: '', zip: '' };
        this.load();
      });
  }
}
