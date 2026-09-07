import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { OperacionesService } from './operaciones.service';
import { OperacionListItem } from './operaciones.model';

@Component({
  selector: 'app-operaciones',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './operaciones.component.html',
  styleUrl: './operaciones.component.css'
})
export class OperacionesComponent implements OnInit {
  private servicio = inject(OperacionesService);

  desde = '2025-01-01';
  hasta = '2025-03-31';

  // TODO: aquí van el filtro por estado y el control de la paginación

  cargando = signal(false);
  error = signal<string | null>(null);
  operaciones = signal<OperacionListItem[]>([]);
  total = signal(0);

  ngOnInit(): void {
    this.buscar();
  }

  buscar(): void {
    this.cargando.set(true);
    this.error.set(null);

    this.servicio.listar({ desde: this.desde, hasta: this.hasta }).subscribe({
      next: (resp) => {
        this.operaciones.set(resp.items);
        this.total.set(resp.total);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudo cargar el listado. ¿Está corriendo la API?');
        this.cargando.set(false);
      }
    });
  }

  totalPagina(): number {
    return this.operaciones().reduce((acc, o) => acc + o.totalCop, 0);
  }
}
