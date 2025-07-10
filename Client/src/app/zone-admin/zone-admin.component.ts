import { Component, OnInit } from '@angular/core';
import { Zone } from '../Models/zone';
import { log } from 'console';


@Component({
  selector: 'app-zone-admin',
  templateUrl: './zone-admin.component.html',
  styleUrls: ['./zone-admin.component.css']
})
export class ZoneAdminComponent implements OnInit {
  ngOnInit(): void {
    this.enableViewAllZone=false;
    this.viewZone="Show all zones";
  }
  searchId: string = '';
  newZone: Zone = { id: '', name: '', description: '',enabled:false };
  enableViewAllZone:any=false
  viewZone:any
  zones: Zone[] = [
    { id: 'Z01', name: 'Zone A', description: ' Campus 30 Near Swing area' ,enabled:true},
    { id: 'Z02', name: 'Zone B', description: 'Campus 30 Near reception' ,enabled:true}
  ];

  filteredZones: Zone[] = [];


  toggleView(){
    this.enableViewAllZone=!this.enableViewAllZone
    if(this.enableViewAllZone){
      this.viewZone="Hide All zones"
    }else{
      this.viewZone="Show All Zones"
    }
  }
  onSearch() {
    const term = this.searchId.toLowerCase();
    this.filteredZones = this.zones.filter(zone =>
      zone.id.toLowerCase().includes(term) || zone.name.toLowerCase().includes(term)
    );
  }

  saveZone(zone: Zone) {
    const index = this.zones.findIndex(z => z.id === zone.id);
    if (index !== -1) {
      this.zones[index] = { ...zone };
    }
  }

  deleteZone(zone: Zone) {
    this.zones = this.zones.filter(z => z.id !== zone.id);
    this.filteredZones = this.filteredZones.filter(z => z.id !== zone.id);
  }

  addZone() {
    if (this.newZone.id && this.newZone.name) {
      this.zones.push({ ...this.newZone });
      this.newZone = { id: '', name: '', description: '',enabled:false };
    }
    console.log(this.zones);
    
  }
}
