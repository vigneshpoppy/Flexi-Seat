export interface Zone{
  id: string;          // Unique zone code like "Z01"
  name: string;        // Zone name like "Zone A"
  description: string; // Description like "Region A near pantry"
  enabled: boolean;
}

export interface PostZone {
  id: number;
  name: string;
  description?: string;
  locationName?: string;
  isActive?: boolean;
  managerADID?: string;
  managerName?: string;
}