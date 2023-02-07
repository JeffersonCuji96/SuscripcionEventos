import { PersonaDto } from "./personaDto";

export interface UsuarioDto {
    Id: number,
    Email: string,
    Clave: string,
    ImageBase64: string,
    Persona: {
        [key: string]: PersonaDto
    };
}