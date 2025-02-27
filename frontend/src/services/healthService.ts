// src/services/healthService.ts
export async function checkHealth(url: string): Promise<boolean> {
    try {
      const response = await fetch(url);
      return response.ok;
    } catch (error) {
      return false;
    }
  }
  