export interface Role {
  id: number;
  name: string;       // e.g., "Manager", "Team Lead", "HR"
  description: string;
   isActive:boolean;
}

export interface PostRole {
  id?: string;
  name: string;       
  description?: string;
  isActive:boolean;
}