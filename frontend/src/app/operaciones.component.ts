import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal, computed } from '@angular/core';
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

  // Filtro por estado y paginación.
  estado = '';
  estadosDisponibles = signal<string[]>([]);

  pagina = signal(1);
  pageSize = 20;

  cargando = signal(false);
  error = signal<string | null>(null);
  operaciones = signal<OperacionListItem[]>([]);
  total = signal(0);

  // Derivado del total real que devuelve el backend, no de lo que trae la página.
  totalPaginas = computed(() => Math.max(1, Math.ceil(this.total() / this.pageSize)));

  ngOnInit(): void {
    this.servicio.estados().subscribe({
      next: (lista) => this.estadosDisponibles.set(lista),
      error: () => this.estadosDisponibles.set([])
    });

    this.buscar();
  }

  /** Se llama al enviar el formulario de filtros: siempre vuelve a la página 1. */
  buscar(): void {
    this.pagina.set(1);
    this.cargar();
  }

  private cargar(): void {
    this.cargando.set(true);
    this.error.set(null);

    this.servicio
      .listar({
        desde: this.desde,
        hasta: this.hasta,
        estado: this.estado || undefined,
        page: this.pagina(),
        pageSize: this.pageSize
      })
      .subscribe({
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

  paginaAnterior(): void {
    if (this.pagina() <= 1) return;
    this.pagina.set(this.pagina() - 1);
    this.cargar();
  }

  paginaSiguiente(): void {
    if (this.pagina() >= this.totalPaginas()) return;
    this.pagina.set(this.pagina() + 1);
    this.cargar();
  }

  totalPagina(): number {
    return this.operaciones().reduce((acc, o) => acc + o.totalCop, 0);
  }
}