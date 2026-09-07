import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { FiltroOperaciones, OperacionListResponse } from './operaciones.model';

@Injectable({ providedIn: 'root' })
export class OperacionesService {
  private http = inject(HttpClient);

  listar(filtro: FiltroOperaciones): Observable<OperacionListResponse> {
    let params = new HttpParams();

    if (filtro.desde) params = params.set('desde', filtro.desde);
    if (filtro.hasta) params = params.set('hasta', filtro.hasta);
    if (filtro.estado) params = params.set('estado', filtro.estado);
    if (filtro.page) params = params.set('page', filtro.page);
    if (filtro.pageSize) params = params.set('pageSize', filtro.pageSize);

    return this.http.get<OperacionListResponse>('/api/operaciones', { params });
  }

  estados(): Observable<string[]> {
    return this.http.get<string[]>('/api/operaciones/estados');
  }
}