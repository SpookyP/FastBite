using MenuCatalog.Application.IService;
using MenuCatalog.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

namespace MenuCatalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenusController(IMenuService menuService) : ControllerBase
    {
        private readonly IMenuService _menuService = menuService;


        [HttpGet("ObterPorId")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido. O ID deve ser maior do que zero.");
            }

            var menuId = await _menuService.ObterPorIdAsync(id);

            if (menuId == null)
            {
                return NotFound("Menu não encontrado.");
            }

            return Ok(menuId);

        }

        [HttpDelete("Eliminar")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoverMenu(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido. O ID deve ser maior do que zero.");
            }

            var menuExistente = await _menuService.ObterPorIdAsync(id);
            if (menuExistente == null)
            {
                return NotFound("Menu não encontrado.");
            }

            await _menuService.RemoverMenuAsync(id);
            return NoContent(); //204 - O servidor respondeu ao pedido, mas não retorna nenhum conteúdo.
        }

        [HttpGet("VerDisponibilidade")]
        public async Task<IActionResult> VerDisponibilidade(int id, [FromQuery] int quantidade) //FromQuery - 'ensina' o controller a ler tudo o que vem depois do ? no URL (?quantidade=5)
        {
            if (id <= 0 || quantidade <= 0)
            {
                return BadRequest("O ID do prato e a quantidade devem ser maiores que zero.");
            }

            var qntDisponivel = await _menuService.VerDisponibilidadeAsync(id, quantidade);

            return Ok(qntDisponivel);
        }
        
    }
}
