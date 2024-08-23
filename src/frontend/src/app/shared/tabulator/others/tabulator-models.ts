export interface TabulatorQuery {
  size: number;
  page: number;
  filter?: TabulatorFilter;
  sort?: TabulatorSort;
}

export interface TabulatorFilter {
  field: string;
  type: number;
  value: string;
}
export interface TabulatorSort {
  field: string;
  dir: 'asc' | 'desc';
}
