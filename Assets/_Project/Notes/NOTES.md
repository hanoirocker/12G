NOVEDADES:

- Se corrigió tamaño de interacción de colliders de puertas.
- Se redujo el brillo de la luz del prefab del teléfono del inventario.

TOCHECK:

- HEADACHES: actualmente se terminó el final de la primera parte, pero queda aún se podría mencionar al inicio de Headaches el dolor de cabeza que le ocasiona el walkie talkie y los aparatos electrónicos. Quizas luego de eso podriamos prender y apagar aparatos aleatoriamente? (microondas, radio, luces de la casa)
- Agregar mas sonidos stereo para los perfiles sonoros de ambiente?
- Por alguna razón, al contemplar la escoba ya usada "Used Broom" no devuelve texto. (Solucionado pero hay que checkear que los indexes nuevos sean correctos)

TOFIX:

- No había audio (voces) después de progresar linealmente al first contact. Tuve que cargar el evento manualmente
- Agregar luz area baked al interior de la caja fuerte (la luz puntal realtime se nota como reflejo en el material)
- Used Broom: cambiar collider circular por rectangular.
- Control Canvas: agregar control principal de Menu Pausa / Pistas [Esc]
- Pizza time: agregar posibilidad de tomar plato de la pila del estante. Si se toma de uno, se inhabilita el collider del otro desde sus scripts con método extra.
- Birds: colocar collider de pajaro que choca contra ventana un poco antes (entre middle stairs y upper stairs).
- Birds: modificar texto al intentar limpiar ave (Indicar que la bolsa y escoba están en el garage) --> Mover escoba al garage. También modificar texto del player helper data indicando lo del garage.
- Revisar cuadros examinables, hay uno que la pata trasera no se renderiza en overlay.
- Revisar UI de examinación (estilos).
- Al empezar el afternoon scene desde el evento PizzaTime, no hay plato colgado para levantar.
- Al empezar el afternoon scene desde cualquier evento posterior a PizzaTime, la escoba está en el garage y no aparece la escoba usada.
- LS2: deshabilitar los controles del jugador apenas se enciende la PC, retomarlos al salir de la misma. Por otro lado, Al terminar de usar la misma debemos mostrar solo el cartel de LEVANTARSE []. Luego una vez recuperados los controles y estando parado, se debe poder apagar.
- Si habilitamos el Walkie Talkie mediante un checkpoint asset, se rompe al mostrarlo o intentar cambiar el canal debido a que no genera una estructura de canales --> Esto sucede cuando no existe extructura de audios de canales para el evento en particular.

TODO - próximo:

- Al iniciar el evento "Red Hour" los cambios cambian de golpe antes de empezar la lógica de glowing, pudiendo ser observado este cambio por el jugador. Buscar la forma de que esto no suceda (Quizas agregar algo al evento al incio, antes del glowing?)
- Componer audio para ScaredReactionLong // LISTO PERO NO FINAL
- Actualmente el sistema de audio por perfiles de ambiente funciona en base a la escena. Por ende, si quisieramos detener la lluvia en algun momento, la lluvia estereo del los perfiles seguiria sonando. Analizar si es conveniente o no ampliar la estructura logica.
- TIPS LOGIC: Nueva idea. Si el jugador presiona una tecla asociada INDICADA como comando extra en opciones principales de canvas de control, se abre un nuevo canvas TIPS CANVAS sobre la esquina inferior derecha que indica lo que diría el Player Helper Data.

TODO - despues:

- Verificar si es necesario cambiar texto de observación apenas comienza Walkie Talkie Quest para dejar en claro que hablar con Mica NO ES SALIR DE LA CASA.
- Poblar la heladera.
- Agregar interacción con Click Izq (además de botón [E], pero solo para interacciones con objetos, no en animaciones, etc ..)
- Revisar objeto de bolsa de basura. Considerar colgar una bolsa con un simbolo que represente basura de la cual se extraiga una bolsa (agregar sonido de bolsa de nylon tomada).
- Walkie Talkie: Agregar ruidos de estática distintos para cada canal (En todos los assets de WalkieTalkieData).
- Quizás trabajar un poco mas sobre el audio de la alucinación de fernandez // LISTO PERO NO FINAL
- Agregar risas femeninas al audio del cuadro del corazón.
- Agregar sonidos para cuando come la pizza.
- Agregar sonidos al tomar bolsas de basura.
- Agregar sonido de mareo y caida del jugador al suelo, dependiendo del lugar donde caiga debe sonar distinto (3 materiales: carpet, wood, mosaic)
- Agregar animación de primera activación de linterna para Unwired? (MAYBE)
- Componer distintos audios según intensidad de Electric Feel. La idea es que sean clips conformados por el sonido actual (loopeable), luego un clip que incluya el layer anterior mas un fade in del nuevo layer de audio (Play one shot), y finalmente el nuevo audio (loopeable).
- Agregar sonido para animación "Night - Stand Up".
- Agregar sonido para animación "Night - Unwired - Look For Flashlight" de FlashlightVC

- Agregar cajas bajo la escalera, o algo.

- Actualizar textos en puertas principales y objetos contemplables al final de la escena de Evening. Por ahora se han dejado de trabajar los SO's.
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

- Eliminar evento de gatito. Ahora se abre la puerta del garage luego de salir de la piecita de la cocina y es el momento de salir de la casa por el garage.

- CUALQUIER EVENTO DE LA NOCHE: la idea de que al entrar a la oficina del padre el jugador se entera de un secreto choto (ejemplo, cuenta de tinder abierta del padre) + posible sonido de enemigo golpeando su ventana y riendo.
- RECORDAR AGREGAR INTERACTION CON CAJONES Y ASSETS PARA POBLAR ESTOS.

- Something Else: pasillo de la escuela.
  1. Si el jugador luego de "X" cantidad de loops cuando empieza a crecer la luz roja y se acerca el enemigo decide quedarse quieto (jumpscare, bla).
  2. Si corre hasta el final --> animación de que se salva.
- Post Evening Scene (Nueva)? - Se levanta, escucha ruidos de Micaela que le indican que la está pasando mal y sale de la casa (sol naranja que lo destella (Cinematica a partir de un punto en la entrada de la casa)). FIN DEL JUEGO --> Creditos.
