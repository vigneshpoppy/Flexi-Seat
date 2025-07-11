export interface Employee {
  id: string;          // AI ID or employee code
  name: string;
  position: string;
  manager: string;
  location: string;
}

export interface User {
  adid: string;
  name: string;
  designation: string;
  badgeId: string;
  roleId: number;
  leadADID?: string | null;
  managerADID?: string | null;
}

export interface UserPatch {
  
  name: string;
  designation: string;
  badgeId: string;
  roleId: number;
  leadADID?: string | null;
  managerADID?: string | null;
}