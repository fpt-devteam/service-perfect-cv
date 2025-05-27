using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Item.Requests;
using ServicePerfectCV.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Route("api/items")]
    public class ItemController(ItemService itemService) : ControllerBase
    {

        [HttpPut("{itemId}")]
        public async Task<IActionResult> UpdateAsync(Guid itemId, [FromBody] ItemUpdateRequest request)
        {

            await itemService.UpdateItemAsync(itemId, request);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await itemService.DeleteItemAsync(id);
            return NoContent();
        }



    }
}