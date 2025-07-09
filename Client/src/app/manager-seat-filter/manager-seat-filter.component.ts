import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-manager-seat-filter',
  templateUrl: './manager-seat-filter.component.html',
  styleUrls: ['./manager-seat-filter.component.css']
})
export class ManagerSeatFilterComponent {
  managers = [
    { name: 'Ruso', regions: ['B', 'C'] },
    { name: 'Rupinder', regions: ['A', 'D'] },
    { name: 'Rajesh', regions: ['E', 'F'] }
  ];

  @Output() filterRegions = new EventEmitter<string[]>();
   @Output() manager = new EventEmitter<string>();

  selectedManager: string = '';

  onManagerSelect(managerName: string) {
    const manager = this.managers.find(m => m.name === managerName);
    if (manager) {
      console.log("Manger Component: "+ manager.name);
      console.log(manager.regions);
      
      this.filterRegions.emit(manager.regions);
      this.manager.emit(manager?.name);
      
    }
  }
}
