import { Component, OnInit } from '@angular/core';
import { Employee } from '../Models/employee';
import { AuthServiceService } from '../Service/auth-service.service';

@Component({
  selector: 'app-employee-admin',
  templateUrl: './employee-admin.component.html',
  styleUrls: ['./employee-admin.component.css']
})
export class EmployeeAdminComponent  implements OnInit{

  constructor(private authservice:AuthServiceService){}
  isAdmin:any= false;
  ngOnInit(): void {
    this.getRoles();
  }

  getRoles(){
   
    if(this.authservice.hasRole("Admin")){
      this.isAdmin=true;
    }
  }
  searchId: string = '';
  newEmployee: Employee = { id: '', name: '', position: '', manager: '', location: '' };

  employees: Employee[] = [
    { id: 'YJJQQQ', name: 'Vignesh', position: 'Application Developer', manager: 'Ruso', location: 'Chennai' },
    { id: 'LFH1WW', name: 'Ruso', position: 'Lead Application Developer', manager: 'Rupinder', location: 'Chennai' }
  ];

  filteredEmployees: Employee[] = [];

  onSearch() {
    this.filteredEmployees = this.employees.filter(emp =>
      emp.id.toLowerCase().includes(this.searchId.toLowerCase())
    );
  }

  saveEmployee(emp: Employee) {
    const index = this.employees.findIndex(e => e.id === emp.id);
    if (index !== -1) {
      this.employees[index] = { ...emp };
    }
  }

  deleteEmployee(emp: Employee) {
    this.employees = this.employees.filter(e => e.id !== emp.id);
    this.filteredEmployees = this.filteredEmployees.filter(e => e.id !== emp.id);
  }

  addEmployee() {
    if (this.newEmployee.id && this.newEmployee.name) {
      this.employees.push({ ...this.newEmployee });
      this.newEmployee = { id: '', name: '', position: '', manager: '', location: '' };
    }
  }
}