export interface TagListDTO {
  id: string;
  name: string;
  createdAtUtc: Date;
  updatedAtUtc?: Date;
}
  
export interface TagCreateDTO {
  name: string;
}

export interface TagUpdateDTO {
  name: string;
}