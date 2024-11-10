export interface TokenDto {
    id: string,
    token: string,
    provider: string,
    is2FaRequired: boolean
}