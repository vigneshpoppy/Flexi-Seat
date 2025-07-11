import { Component, OnInit } from '@angular/core';
import { Employee, User, UserPatch } from '../Models/employee';
import { AuthServiceService } from '../Service/auth-service.service';
import { EmployeeService } from '../Service/employee.service';
import { NotificationService } from '../Service/notification.service';
import { log } from 'node:console';
import { RoleServiceService } from '../Service/role-service.service';

@Component({
  selector: 'app-employee-admin',
  templateUrl: './employee-admin.component.html',
  styleUrls: ['./employee-admin.component.css']
})
export class EmployeeAdminComponent  implements OnInit{

  constructor(private authservice:AuthServiceService,private employeeservice:EmployeeService,
    private notify:NotificationService,private roleService:RoleServiceService){}
  isAdmin:any= false;
  allRoles:any
  ngOnInit(): void {
    this.getRoles();
    this.getAllRoles();
  }

  getRoles(){
   
    if(this.authservice.hasRole("admin")){
      this.isAdmin=true;
    }
  }
  searchId: string = '';
  newEmployee: User = { adid: '', name: '', designation: '', badgeId: '',roleId:0 ,managerADID: '' };

  employees: User[] = [
    // { id: 'YJJQQQ', name: 'Vignesh', position: 'Application Developer', manager: 'Ruso', location: 'Chennai' },
    // { id: 'LFH1WW', name: 'Ruso', position: 'Lead Application Developer', manager: 'Rupinder', location: 'Chennai' }
  ];

  filteredEmployees: User[] = [];
  getAllRoles(){
    this.roleService.getRoleAllData().subscribe({
      next:result=>{
    this.allRoles= result
    console.log(this.allRoles);

      },
      error:err=>{
        console.log(err);
        
      }
    })
  }
  onSearch() {
   const term = this.searchId.toUpperCase();

   this.employeeservice.getRoleByID(term).subscribe({
    next: result => {
      console.log(result)
      this.filteredEmployees = result ? [result] : [];
      console.log(this.filteredEmployees);
    },
    error: err => {
      console.error('Error fetching User by ID:', err);
      this.filteredEmployees = [];
    }
  });
  }

  saveEmployee(emp: User) {

    console.log(emp);
    
    const payload: UserPatch = { name: emp.name, designation: emp.designation, badgeId:emp.badgeId,
      roleId:emp.roleId ,managerADID: emp.managerADID,leadADID:emp.leadADID };
    this.employeeservice.updateRole(emp.adid,payload).subscribe({
      next: result =>{
        this.notify.showSuccess("employee Updated")
        console.log("employee Updated");
        
      },
      error:err=>{
        this.notify.showError(err);
      console.log(err);

      }
    })
  }
  

  deleteEmployee(emp: User) {
       this.employeeservice.deleteRole(emp.adid).subscribe({
      next: res=>{
        this.notify.showSuccess("User Deleted")
        console.log(res);
        this.filteredEmployees= [];

        
      },error:err=>{
          this.notify.showError(err);
        console.log(err);
        
      }
     })
  }

  addEmployee() {
    if (this.newEmployee.adid && this.newEmployee.name) {
     console.log({...this.newEmployee});
     const managerId=this.newEmployee.managerADID==""?null:this.newEmployee.managerADID;
          const LeadId=this.newEmployee.leadADID==""?null:this.newEmployee.leadADID;
          
      const payload: User = { adid:this.newEmployee.adid,name:this.newEmployee.name, designation: this.newEmployee.designation, badgeId:this.newEmployee.badgeId,
      roleId:this.newEmployee.roleId ,managerADID: managerId,leadADID:LeadId};

      console.log("PostPaylod");
      console.log(payload);
      
      this.employeeservice.postRoleData(payload).subscribe({
        next :result=>{
         this.notify.showSuccess("user Added")
          this.newEmployee = { adid: '', name: '', designation: '', badgeId: '',roleId:0 ,managerADID: '' };
        
        },
        error:err=>{
            this.notify.showError(err);
          console.log(err);
          
        }
       
        
      })
      this.newEmployee = { adid: '', name: '', designation: '', badgeId: '',roleId:0 ,managerADID: '' };
    }
  }
}