import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AzureAIService {

  private localBaseUrl=`${environment.apiUrl}/api/Bot/`
  private apiUrl = 'https://flexiseatai.openai.azure.com/openai/deployments/gpt-4.1/chat/completions?api-version=2025-01-01-preview';
  private apiKey = "";

  constructor(private http: HttpClient) {}

  async askAzureAI(userMessage: string): Promise<string> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'api-key': this.apiKey
    });

    const body = {
      messages: [
        { role: 'user', content: userMessage }
      ],
      temperature: 0.7
    };

    try {
      const response: any = await this.http.post(this.apiUrl, body, { headers }).toPromise();
      return response.choices[0].message.content;
    } catch (err) {
      console.error('Azure AI error:', err);
      return "Sorry, I couldn't process that.";
    }
  }

  async OpenAICall(userid:string,userinput:string): Promise<Observable<any>> {
        return this.http.post(`${this.localBaseUrl}`, userinput);
      }
}
