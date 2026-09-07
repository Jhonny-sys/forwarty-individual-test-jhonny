export interface OperacionListItem {
  id: number;
  numeroOperacion: string;
  clienteRazonSocial: string;
  tipo: string;
  modalidad: string;
  estado: string;
  fechaApertura: string;
  totalCop: number;
  cantidadCostos: number;
}

export interface OperacionListResponse {
  items: OperacionListItem[];
  /** Cantidad de operaciones que cumplen los filtros. */
  total: number;
}

/** Filtros que viajan al backend. */
export interface FiltroOperaciones {
  desde?: string;
  hasta?: string;
  // TODO: falta un campo más para el filtro por estado
}
