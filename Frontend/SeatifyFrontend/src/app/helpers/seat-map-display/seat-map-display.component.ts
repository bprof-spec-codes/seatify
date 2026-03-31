import {
  Component,
  ElementRef,
  ViewChild,
  AfterViewInit,
  OnDestroy
} from '@angular/core';
import * as THREE from 'three';

@Component({
  selector: 'app-seat-map-display',
  standalone: false,
  templateUrl: './seat-map-display.component.html',
  styleUrls: ['./seat-map-display.component.sass']
})
export class SeatMapDisplayComponent implements AfterViewInit, OnDestroy {
  @ViewChild('canvasContainer', { static: true })
  canvasContainer!: ElementRef<HTMLDivElement>;

  private scene!: THREE.Scene;
  private camera!: THREE.PerspectiveCamera;
  private renderer!: THREE.WebGLRenderer;
  private animationFrameId: number | null = null;
  private timer = new THREE.Timer();

  private seatGroups: THREE.Group[] = [];
  private stageGlow!: THREE.Mesh;

  private raycaster = new THREE.Raycaster();
  private mouse = new THREE.Vector2(-2, -2);
  private interactableMeshes: THREE.Mesh[] = [];
  private hoveredSeat: THREE.Group | null = null;
  private allSeats: THREE.Group[] = [];

  ngAfterViewInit(): void {
    requestAnimationFrame(() => {
      this.initThree();
      this.buildScene();
      this.animate();
    });
  }

  ngOnDestroy(): void {
    if (this.animationFrameId !== null) {
      cancelAnimationFrame(this.animationFrameId);
    }

    window.removeEventListener('resize', this.onResize);

    const container = this.canvasContainer?.nativeElement;
    if (container) {
      container.removeEventListener('mousemove', this.onMouseMove);
      container.removeEventListener('mouseleave', this.onMouseLeave);
    }

    if (this.renderer) {
      this.renderer.dispose();
    }
  }

  private initThree(): void {
    const container = this.canvasContainer.nativeElement;

    this.scene = new THREE.Scene();
    this.scene.fog = new THREE.Fog(0xf3f7ff, 55, 120);

    this.camera = new THREE.PerspectiveCamera(
      42,
      container.clientWidth / container.clientHeight,
      0.1,
      300
    );

    this.camera.position.set(0, 18, 34);
    this.camera.lookAt(0, 7, -2);

    this.renderer = new THREE.WebGLRenderer({
      antialias: true,
      alpha: true,
      powerPreference: 'high-performance'
    });

    this.renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
    this.renderer.setSize(container.clientWidth, container.clientHeight);
    this.renderer.setClearColor(0xffffff, 0);
    this.renderer.shadowMap.enabled = true;
    this.renderer.shadowMap.type = THREE.PCFShadowMap;
    this.renderer.toneMapping = THREE.ACESFilmicToneMapping;
    this.renderer.toneMappingExposure = 1.08;
    this.renderer.outputColorSpace = THREE.SRGBColorSpace;

    container.appendChild(this.renderer.domElement);

    window.addEventListener('resize', this.onResize);
    container.addEventListener('mousemove', this.onMouseMove);
    container.addEventListener('mouseleave', this.onMouseLeave);
  }

  private buildScene(): void {
    const ambient = new THREE.HemisphereLight(0xffffff, 0xdbe7ff, 1.45);
    this.scene.add(ambient);

    const sun = new THREE.DirectionalLight(0xffffff, 1.25);
    sun.position.set(18, 30, 22);
    sun.castShadow = true;
    sun.shadow.mapSize.set(2048, 2048);
    sun.shadow.camera.near = 1;
    sun.shadow.camera.far = 150;
    sun.shadow.camera.left = -40;
    sun.shadow.camera.right = 40;
    sun.shadow.camera.top = 40;
    sun.shadow.camera.bottom = -40;
    this.scene.add(sun);

    const fill = new THREE.PointLight(0x60a5fa, 18, 120, 2);
    fill.position.set(-18, 12, 18);
    this.scene.add(fill);

    const warm = new THREE.PointLight(0xf59e0b, 10, 90, 2);
    warm.position.set(18, 10, 16);
    this.scene.add(warm);

    const floor = new THREE.Mesh(
      new THREE.CircleGeometry(70, 96),
      new THREE.MeshStandardMaterial({
        color: 0xf8fbff,
        roughness: 0.96,
        metalness: 0.02
      })
    );
    floor.rotation.x = -Math.PI / 2;
    floor.receiveShadow = true;
    this.scene.add(floor);

    const floorHalo = new THREE.Mesh(
      new THREE.CircleGeometry(48, 96),
      new THREE.MeshBasicMaterial({
        color: 0x93c5fd,
        transparent: true,
        opacity: 0.12
      })
    );
    floorHalo.rotation.x = -Math.PI / 2;
    floorHalo.position.y = 0.02;
    this.scene.add(floorHalo);

    this.createStage();
    this.createRows();
  }

  private createStage(): void {
    const stageBase = new THREE.Mesh(
      new THREE.CylinderGeometry(11.5, 12.2, 1.4, 96),
      new THREE.MeshPhysicalMaterial({
        color: 0xffffff,
        roughness: 0.2,
        metalness: 0.04,
        clearcoat: 0.6
      })
    );
    stageBase.position.set(0, 0.75, 0);
    stageBase.castShadow = true;
    stageBase.receiveShadow = true;
    this.scene.add(stageBase);

    const stageTop = new THREE.Mesh(
      new THREE.CylinderGeometry(10.3, 10.8, 0.24, 96),
      new THREE.MeshPhysicalMaterial({
        color: 0xeaf2ff,
        roughness: 0.14,
        metalness: 0.03,
        clearcoat: 0.75
      })
    );
    stageTop.position.set(0, 1.58, 0);
    stageTop.castShadow = true;
    stageTop.receiveShadow = true;
    this.scene.add(stageTop);

    this.stageGlow = new THREE.Mesh(
      new THREE.TorusGeometry(10.8, 0.18, 16, 120),
      new THREE.MeshBasicMaterial({
        color: 0x3b82f6,
        transparent: true,
        opacity: 0.5
      })
    );
    this.stageGlow.rotation.x = Math.PI / 2;
    this.stageGlow.position.y = 1.72;
    this.scene.add(this.stageGlow);
  }

  private createRows(): void {
    const materials = {
      vip: new THREE.MeshPhysicalMaterial({
        color: 0xf59e0b,
        roughness: 0.35,
        metalness: 0.05,
        clearcoat: 0.3
      }),
      premium: new THREE.MeshPhysicalMaterial({
        color: 0x22c55e,
        roughness: 0.32,
        metalness: 0.05,
        clearcoat: 0.32
      }),
      regular: new THREE.MeshPhysicalMaterial({
        color: 0x3b82f6,
        roughness: 0.28,
        metalness: 0.05,
        clearcoat: 0.36
      })
    };

    const totalRows = 7;
    const baseRadius = 15.5;
    const radiusStep = 4.1;

    const baseRowY = 1.9;
    const rowYStep = 1.35;

    const spread = Math.PI * 0.94;
    const aisleHalfAngle = 0.11;
    const seatSpacing = 3.6;

    for (let row = 0; row < totalRows; row++) {
      const radius = baseRadius + row * radiusStep;
      const rowY = baseRowY + row * rowYStep;

      let material = materials.vip;
      if (row >= 2 && row <= 4) material = materials.premium;
      if (row >= 5) material = materials.regular;

      this.createRowPlatform(radius, rowY - 0.02, spread);

      const rowGroup = new THREE.Group();

      const usableHalfSpread = spread / 2 - aisleHalfAngle;
      const angleStep = seatSpacing / radius;
      const seatsPerSide = Math.max(4, Math.floor(usableHalfSpread / angleStep));

      for (const side of [-1, 1]) {
        for (let i = 0; i < seatsPerSide; i++) {
          const offset = aisleHalfAngle + angleStep * (i + 0.5);
          const angle = Math.PI / 2 + side * offset;

          const seat = this.createSeat(material);

          const x = Math.cos(angle) * radius;
          const z = -Math.sin(angle) * radius;

          seat.position.set(x, rowY, z);
          seat.lookAt(0, rowY + 0.2, 0);

          seat.userData = { targetY: rowY, initialY: rowY };
          this.allSeats.push(seat);

          rowGroup.add(seat);
        }
      }

      this.scene.add(rowGroup);
      this.seatGroups.push(rowGroup);
    }
  }

  private createRowPlatform(radius: number, y: number, spread: number): void {
    const thetaStart = Math.PI / 2 - spread / 2;
    const thetaLength = spread;

    const platform = new THREE.Mesh(
      new THREE.RingGeometry(radius - 1.45, radius + 1.15, 96, 1, thetaStart, thetaLength),
      new THREE.MeshStandardMaterial({
        color: 0xe5e7eb,
        roughness: 0.92,
        metalness: 0.02,
        side: THREE.DoubleSide
      })
    );

    platform.rotation.x = -Math.PI / 2;
    platform.position.y = y;
    platform.receiveShadow = true;
    this.scene.add(platform);

    const edge = new THREE.Mesh(
      new THREE.RingGeometry(radius + 1.02, radius + 1.14, 96, 1, thetaStart, thetaLength),
      new THREE.MeshBasicMaterial({
        color: 0xcbd5e1,
        transparent: true,
        opacity: 0.8,
        side: THREE.DoubleSide
      })
    );

    edge.rotation.x = -Math.PI / 2;
    edge.position.y = y + 0.01;
    this.scene.add(edge);
  }

  private createSeat(material: THREE.MeshPhysicalMaterial): THREE.Group {
    const group = new THREE.Group();

    const frameMaterial = new THREE.MeshStandardMaterial({
      color: 0x94a3b8,
      roughness: 0.55,
      metalness: 0.4
    });

    const seatBase = new THREE.Mesh(
      new THREE.BoxGeometry(2.1, 0.36, 1.9),
      material
    );
    seatBase.position.set(0, 0.82, 0.2);
    seatBase.castShadow = true;
    seatBase.receiveShadow = true;

    const seatBack = new THREE.Mesh(
      new THREE.BoxGeometry(2.1, 1.8, 0.3),
      material
    );
    
    seatBase.userData = { parentGroup: group };
    seatBack.userData = { parentGroup: group };
    this.interactableMeshes.push(seatBase, seatBack);
    seatBack.position.set(0, 1.72, -0.65);
    seatBack.rotation.x = -0.12;
    seatBack.castShadow = true;
    seatBack.receiveShadow = true;

    const leftLegFront = new THREE.Mesh(
      new THREE.CylinderGeometry(0.08, 0.08, 0.8, 10),
      frameMaterial
    );
    leftLegFront.position.set(-0.8, 0.4, 0.55);

    const rightLegFront = leftLegFront.clone();
    rightLegFront.position.x = 0.8;

    const leftLegBack = leftLegFront.clone();
    leftLegBack.position.z = -0.35;

    const rightLegBack = leftLegFront.clone();
    rightLegBack.position.x = 0.8;
    rightLegBack.position.z = -0.35;

    group.add(
      seatBase,
      seatBack,
      leftLegFront,
      rightLegFront,
      leftLegBack,
      rightLegBack
    );

    return group;
  }

  private animate = (): void => {
    this.timer.update();
    this.timer.connect(document);
    const t = this.timer.getElapsed();

    this.camera.position.x = Math.sin(t * 0.28) * 4.5;
    this.camera.position.y = 18 + Math.sin(t * 0.65) * 0.35;
    this.camera.position.z = 34 + Math.cos(t * 0.28) * 1.4;
    this.camera.lookAt(0, 7, -2);

    if (this.stageGlow.material instanceof THREE.MeshBasicMaterial) {
      this.stageGlow.material.opacity = 0.42 + Math.sin(t * 2.1) * 0.08;
    }

    this.raycaster.setFromCamera(this.mouse, this.camera);
    const intersects = this.raycaster.intersectObjects(this.interactableMeshes, false);

    if (intersects.length > 0) {
      const obj = intersects[0].object;
      const group = obj.userData['parentGroup'] as THREE.Group;
      if (this.hoveredSeat !== group) {
        if (this.hoveredSeat) {
          this.hoveredSeat.userData['targetY'] = this.hoveredSeat.userData['initialY'];
        }
        this.hoveredSeat = group;
        if (this.hoveredSeat) {
          this.hoveredSeat.userData['targetY'] = this.hoveredSeat.userData['initialY'] + 0.8;
        }
      }
    } else {
      if (this.hoveredSeat) {
        this.hoveredSeat.userData['targetY'] = this.hoveredSeat.userData['initialY'];
        this.hoveredSeat = null;
      }
    }

    for (let i = 0; i < this.allSeats.length; i++) {
      const seat = this.allSeats[i];
      seat.position.y += (seat.userData['targetY'] - seat.position.y) * 0.15;
    }

    this.renderer.render(this.scene, this.camera);
    this.animationFrameId = requestAnimationFrame(this.animate);
  };

  private onResize = (): void => {
    const container = this.canvasContainer.nativeElement;

    this.camera.aspect = container.clientWidth / container.clientHeight;
    this.camera.updateProjectionMatrix();
    this.renderer.setSize(container.clientWidth, container.clientHeight);
  };

  private onMouseMove = (event: MouseEvent): void => {
    const container = this.canvasContainer.nativeElement;
    const rect = container.getBoundingClientRect();
    
    this.mouse.x = ((event.clientX - rect.left) / container.clientWidth) * 2 - 1;
    this.mouse.y = -((event.clientY - rect.top) / container.clientHeight) * 2 + 1;
  };

  private onMouseLeave = (): void => {
    this.mouse.x = -2;
    this.mouse.y = -2;
    if (this.hoveredSeat) {
      this.hoveredSeat.userData['targetY'] = this.hoveredSeat.userData['initialY'];
      this.hoveredSeat = null;
    }
  };
}