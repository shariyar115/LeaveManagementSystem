import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api.config';
import {
  BulkApproval,
  LeaveApproval,
  LeaveFilter,
  LeaveRequest,
  LeaveSubmission,
} from '../models/leave.models';

@Injectable({ providedIn: 'root' })
export class LeaveRequestService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);
  private get url() {
    return `${this.baseUrl}/leaverequests`;
  }

  getAll(filter: LeaveFilter = {}): Observable<LeaveRequest[]> {
    let params = new HttpParams();
    if (filter.employeeId != null) params = params.set('employeeId', filter.employeeId);
    if (filter.status) params = params.set('status', filter.status);
    if (filter.leaveTypeId != null) params = params.set('leaveTypeId', filter.leaveTypeId);
    if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
    if (filter.toDate) params = params.set('toDate', filter.toDate);
    return this.http.get<LeaveRequest[]>(this.url, { params });
  }

  getPending(): Observable<LeaveRequest[]> {
    return this.http.get<LeaveRequest[]>(`${this.url}/pending`);
  }

  submit(dto: LeaveSubmission): Observable<LeaveRequest> {
    return this.http.post<LeaveRequest>(this.url, dto);
  }

  setApproval(id: number, dto: LeaveApproval): Observable<void> {
    return this.http.post<void>(`${this.url}/${id}/approval`, dto);
  }

  bulkApproval(dto: BulkApproval): Observable<void> {
    return this.http.post<void>(`${this.url}/bulk-approval`, dto);
  }

  cancel(id: number): Observable<void> {
    return this.http.post<void>(`${this.url}/${id}/cancel`, {});
  }
}
