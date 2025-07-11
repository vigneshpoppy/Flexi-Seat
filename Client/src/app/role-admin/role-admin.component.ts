import { Component, OnInit } from '@angular/core';
import { Role } from '../Models/role';
import { RoleServiceService } from '../Service/role-service.service';
import { error, log } from 'console';
import { NotificationService } from '../Service/notification.service';


@Component({
  selector: 'app-role-admin',
  templateUrl: './role-admin.component.html',
  
  styleUrls: ['./role-admin.component.css']
})
export class RoleAdminComponent  implements OnInit{

  constructor(private roleService:RoleServiceService,private notify:NotificationService){

  }

   ngOnInit(): void {
    this.GetAllRoles();
      this.enableViewAllRole=false;
      this.viewRole="Show all Roles";
    }
  
    enableViewAllRole:any=false
    viewRole:any
 
  searchId: string = '';
  newRole: Role = { id: 0, name: '', description: '',isActive:true };

  roles: Role[] = [
    // { id: , name: 'Manager', description: 'Manages a team',isActive:true },
    // { id: 'R02', name: 'Team Lead', description: 'Leads development team',isActive:true }
  ];

  filteredRoles: Role[] = [];

  toggleView(){
    this.enableViewAllRole=!this.enableViewAllRole
    if(this.enableViewAllRole){
      this.viewRole="Hide All Roles"
    }else{
        this.GetAllRoles();
      this.viewRole="Show All Roles"
    }
  }
  GetAllRoles(){
    this.roleService.getRoleAllData().subscribe(x=>{
     this.roles=x
     console.log(this.roles);
     
    })
  }
  onSearch() {
    const term = this.searchId.toLowerCase();

   this.roleService.getRoleByID(term).subscribe({
    next: result => {
      this.filteredRoles = result ? [result] : [];
    },
    error: err => {
      console.error('Error fetching role by ID:', err);
      this.filteredRoles = [];
    }
  });
  }

  saveRole(role: Role) {

    this.roleService.updateRole(role.id,role).subscribe({
      next: result =>{
        this.notify.showSuccess("Role Updated")
        console.log("Role Updated");
        
      },
      error:err=>{
        this.notify.showError(err);
      console.log(err);

      }
     
    });
    // const index = this.roles.findIndex(r => r.id === role.id);
    // if (index !== -1) {
    //   this.roles[index] = { ...role };
    // }
  }

  deleteRole(role: Role) {
     this.roleService.deleteRole(role.id).subscribe({
      next: res=>{
        this.notify.showSuccess("Role Deleted")
        console.log(res);
        
      },error:err=>{
          this.notify.showError(err);
        console.log(err);
        
      }
     })

    this.roles = this.roles.filter(r => r.id !== role.id);
    this.filteredRoles = this.filteredRoles.filter(r => r.id !== role.id);
  }

  addRole() {
    if (this.newRole.id==0 && this.newRole.name) {
      console.log({...this.newRole});
      
      this.roleService.postRoleData({...this.newRole}).subscribe({
        next :result=>{
         this.notify.showSuccess("Role Added")
           this.newRole = { id: 0, name: '', description: '',isActive:true };
         this.GetAllRoles();
        },
        error:err=>{
            this.notify.showError(err);
          console.log(err);
          
        }
       
        
      })
      
      this.newRole = { id: 0, name: '', description: '',isActive:true };
    }
  }
}
