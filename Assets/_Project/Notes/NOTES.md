Últimos cambios:
- Se reemplazó el uso de TogglePlayerCapsule por EnablePlayerControllers. Esto permite desactivar todos los scripts que permiten interactuar con el jugador SIN apagar el game object player capsule. De esta forma se puede acceder al mismo para cambiar su transform durante cut scenes o cualquier otro momento en el que el jugador no está disponible.
- Se agregó un cut scene para el suicidio de fernandez, además de un nuevo modelo y un video para su televisor.

TOFIX:

- Birds: Modificar textos de "Precisa escoba y bolsa", "Precisa escoba" y "Precisa bolsa". Los individuales deben referir a dónde están específicamente mientras que el plural indica la cocina como lugar general donde encontrar ambos.
- Pizza Time: no aparece el canvas control al estar sentado en la mesa comiendo
- Pizza Time: agrandar el collider de la mesa de forma tal que el jugador si o si vea el cartel de interacción y sea más intuitivo.
- Pizza Time: Agregar comentario luego de que pasa el coche de la policia.
- Lost Signal 1: Backpack, reemplazar interacción "Buscar" por 
"Buscar teléfono".
- Lost Signal 1: Luego de que se haya usado la PC, agregar nuevos
textos de observación para la compu no funcionando.
- Lost Signal 2: Cambiar texto al encontrar el teléfono "Maldito pezado de metal ... " por "Maldito pezado de plástico .."
- Fernandez Suicide: Si la puerta del pasillo a la entrada está abierta al momento de iniciar el cut scene, bloquea la visual de la primera cámara. 

- Los textos de textos internos desaparecen rápido si se pisan uno con el otro.


TODO:
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.
- Implementar cambios de Video del Settings Menu
- Mejorar todo el evento del suicidio. Esto incluye mejoras en el cut scene del mismo, agregar sangre en toda la escena y diálogos de Simon, audio (musica y resolver fuentes de ser usadas).

TOCHECK: