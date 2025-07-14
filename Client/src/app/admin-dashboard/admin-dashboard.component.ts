import { Component } from '@angular/core';
import { ChartConfiguration } from 'chart.js';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent {// Seat Occupancy
  seatOccupancyData: ChartConfiguration<'doughnut'>['data'] = {
    labels: ['Occupied', 'Available'],
    datasets: [{
      data: [765, 235],
      backgroundColor: ['#3b82f6', '#d1fae5'],
    }]
  };

  // Zone Enablement
  zoneStatusData: ChartConfiguration<'pie'>['data'] = {
    labels: ['Active', 'Inactive'],
    datasets: [{
      data: [6, 2],
      backgroundColor: ['#10b981', '#f87171'],
    }]
  };

  // Daily Check-ins
  checkinTrendData: ChartConfiguration<'line'>['data'] = {
    labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri'],
    datasets: [{
      label: 'Daily Check-ins',
      data: [120, 250, 320, 180, 420],
      borderColor: '#6366f1',
      backgroundColor: 'rgba(99,102,241,0.2)',
      tension: 0.4,
      fill: true
    }]
  };

  // Role Activity
  roleStatusData: ChartConfiguration<'pie'>['data'] = {
    labels: ['Active', 'Inactive'],
    datasets: [{
      data: [3, 1],
      backgroundColor: ['#38bdf8', '#fca5a5'],
    }]
  };

  // Zone-wise capacity
  zoneBarData: ChartConfiguration<'bar'>['data'] = {
    labels: ['A', 'B', 'C', 'D', 'E'],
    datasets: [{
      label: 'Total Capacity',
      data: [100, 120, 90, 80, 110],
      backgroundColor: '#a78bfa'
    }, {
      label: 'Occupied',
      data: [70, 110, 45, 60, 85],
      backgroundColor: '#4ade80'
    }]
  };
}