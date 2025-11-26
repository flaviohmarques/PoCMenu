using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenuManagementAPI.Application.Common;
using MenuManagementAPI.Application.DTOs;
using MenuManagementAPI.Application.Services;
using MenuManagementAPI.Domain.Exceptions;

namespace MenuManagementAPI.Presentation.Controllers;

/// <summary>
/// Controller para operações de Menu
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MenuController(IMenuService menuService, ILogger<MenuController> logger) : ControllerBase
{
    private readonly IMenuService _menuService = menuService;
    private readonly ILogger<MenuController> _logger = logger;

    /// <summary>
    /// Obtém todos os menus
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MenuDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MenuDto>>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var menus = await _menuService.GetAllAsync(cancellationToken);
            return Ok(ApiResponse<IEnumerable<MenuDto>>.SuccessResponse(menus, "Menus obtidos com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter menus");
            return StatusCode(500, ApiResponse<IEnumerable<MenuDto>>.ErrorResponse("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Busca menus por nome
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MenuDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MenuDto>>>> Search([FromQuery] string? nome, CancellationToken cancellationToken)
    {
        try
        {
            var menus = await _menuService.SearchByNameAsync(nome ?? string.Empty, cancellationToken);
            return Ok(ApiResponse<IEnumerable<MenuDto>>.SuccessResponse(menus, "Busca realizada com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar menus");
            return StatusCode(500, ApiResponse<IEnumerable<MenuDto>>.ErrorResponse("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Obtém um menu pelo ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<MenuDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MenuDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MenuDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var menu = await _menuService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<MenuDto>.SuccessResponse(menu, "Menu obtido com sucesso"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<MenuDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter menu");
            return StatusCode(500, ApiResponse<MenuDto>.ErrorResponse("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Cria um novo menu
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MenuDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<MenuDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<MenuDto>>> Create([FromBody] CreateMenuDto createMenuDto, CancellationToken cancellationToken)
    {
        try
        {
            var menu = await _menuService.CreateAsync(createMenuDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = menu.Id }, ApiResponse<MenuDto>.SuccessResponse(menu, "Menu criado com sucesso"));
        }
        catch (BusinessValidationException ex)
        {
            return BadRequest(ApiResponse<MenuDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar menu");
            return StatusCode(500, ApiResponse<MenuDto>.ErrorResponse("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Atualiza um menu existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<MenuDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MenuDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<MenuDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MenuDto>>> Update(int id, [FromBody] UpdateMenuDto updateMenuDto, CancellationToken cancellationToken)
    {
        try
        {
            var menu = await _menuService.UpdateAsync(id, updateMenuDto, cancellationToken);
            return Ok(ApiResponse<MenuDto>.SuccessResponse(menu, "Menu atualizado com sucesso"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<MenuDto>.ErrorResponse(ex.Message));
        }
        catch (BusinessValidationException ex)
        {
            return BadRequest(ApiResponse<MenuDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar menu");
            return StatusCode(500, ApiResponse<MenuDto>.ErrorResponse("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Deleta um menu
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _menuService.DeleteAsync(id, cancellationToken);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Menu deletado com sucesso"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar menu");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("Erro interno do servidor"));
        }
    }
}
